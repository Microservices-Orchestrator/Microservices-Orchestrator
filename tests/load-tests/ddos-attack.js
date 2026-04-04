import http from 'k6/http';
import { check } from 'k6';
import { Rate } from 'k6/metrics'; // 1. HATA ÇÖZÜLDÜ
import { generateRandomIp, generateRandomThreatType } from './utils.js';

const errorRate = new Rate('errors');

export const options = {
  scenarios: {
    ddos_attack: {
      executor: 'constant-arrival-rate', // Saniyede sabit sayıda istek atmak için
      rate: 500,                         // Saniyede 500 istek (DDoS simülasyonu)
      timeUnit: '1s',
      duration: '60s',                   // Testin toplam süresi (60 saniye)
      preAllocatedVUs: 100,              
      maxVUs: 500,                       
    },
  },
  thresholds: {
    http_req_duration: ['p(90)<1000'],
  },
};

export default function () {
  // 2. HATA ÇÖZÜLDÜ: Gateway'imizin gerçek portu 5089
  const url = 'http://localhost:5089/api/threats';

  // 3. HATA ÇÖZÜLDÜ: Değişkeni tanımladık
  const threatType = generateRandomThreatType(); 

  const payload = JSON.stringify({
    SourceIp: generateRandomIp(),
    ThreatType: threatType,
    Description: `DDoS simülasyonundan gelen ${threatType} tehdidi.`,
    IsCritical: Math.random() < 0.1, 
  });

  const params = { headers: { 'Content-Type': 'application/json' } };

  const res = http.post(url, payload, params);

  // EFSANE DOKUNUŞ: Rate Limiter 429 fırlatıyor mu onu da ölçüyoruz!
  const checkOutput = check(res, {
    'Başarılı Kayıt (201 Created)': (r) => r.status === 201,
    'Rate Limiter Devrede (429 Too Many Requests)': (r) => r.status === 429,
  });

  errorRate.add(!checkOutput);
}