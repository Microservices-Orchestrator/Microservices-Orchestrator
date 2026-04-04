import http from 'k6/http';
import { check, Rate } from 'k6';

// Testin yapılandırma ayarları

const errorRate = new Rate('errors');

export const options = {
  scenarios: {
    ddos_attack: {
      executor: 'constant-arrival-rate', // Saniyede sabit sayıda istek atmak için
      rate: 500,                         // Saniyede 500 istek (DDoS simülasyonu için yüksek bir değer)
      timeUnit: '1s',
      duration: '60s',                   // Testin toplam süresi (60 saniye)
      preAllocatedVUs: 100,              // Önceden bellekte ayrılacak Sanal Kullanıcı (VU)
      maxVUs: 500,                       // İhtiyaç olursa çıkılabilecek maksimum VU
    },
  },
  thresholds: {
    // İsteklerin %90'ı 1000ms'den kısa sürede cevaplanmalı (DDoS altında daha esnek olabiliriz)
    http_req_duration: ['p(90)<1000'],
    // Hata oranı (yani 201 dışında bir yanıt alma oranı) %10'dan az olmalı
    errors: ['rate<0.10'],
  },
};

export default function () {
  // Gateway'in varsayılan olarak 5000 portunda çalıştığını varsayıyoruz.
  // Eğer Gateway henüz hazır değilse veya farklı bir portta çalışıyorsa bu URL'i güncelleyin.
  // Alternatif olarak, doğrudan ThreatAlertService'i hedeflemek için: 'http://localhost:5065/api/threats'
  const url = 'http://localhost:5000/api/threats';

  // DDoS simülasyonu için rastgele IP ve tehdit verileri üretiyoruz
  // Not: Commit 24'te daha gelişmiş Fake payload üreteçleri ekleyeceğiz.
  const randomIp = `192.168.${Math.floor(Math.random() * 255)}.${Math.floor(Math.random() * 255)}`;
  const threatTypes = ['SQL_Injection', 'XSS_Attack', 'Port_Scan', 'Malware_Infection'];
  const randomThreatType = threatTypes[Math.floor(Math.random() * threatTypes.length)];

  const payload = JSON.stringify({
    SourceIp: randomIp,
    ThreatType: randomThreatType,
    Description: `DDoS simülasyonundan gelen ${randomThreatType} tehdidi.`,
    IsCritical: Math.random() < 0.1, // %10 ihtimalle kritik tehdit
  });

  const params = { headers: { 'Content-Type': 'application/json' } };

  const res = http.post(url, payload, params);

  // ThreatAlertService'in başarılı kayıt için 201 Created dönmesini bekliyoruz
  const checkOutput = check(res, {
    'Durum kodu 201 (Created)': (r) => r.status === 201,
  });

  // Eğer check başarısız olursa (yani 201 dışında bir kod dönerse),
  // bunu kendi özel hata metrimize ekliyoruz.
  errorRate.add(!checkOutput);
}