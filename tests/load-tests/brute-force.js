import http from 'k6/http';
import { check } from 'k6';
import { Rate } from 'k6/metrics'; // Hatanın çözümü: Rate buradan gelir!
import { generateRandomPassword } from './utils.js';

// Testin yapılandırma ayarları
const errorRate = new Rate('errors');

export const options = {
  scenarios: {
    brute_force_attack: {
      executor: 'constant-arrival-rate', // Saniyede sabit sayıda istek atmak için
      rate: 100,                         // Saniyede 100 istek
      timeUnit: '1s',
      duration: '30s',                   // Testin toplam süresi (30 saniye)
      preAllocatedVUs: 50,               // Önceden bellekte ayrılacak Sanal Kullanıcı (VU)
      maxVUs: 300,                       // İhtiyaç olursa çıkılabilecek maksimum VU
    },
  },
  thresholds: {
    // İsteklerin %95'i 500ms'den kısa sürede cevaplanmalı
    http_req_duration: ['p(95)<500'],
    // Hata oranı (yani 401 dışında bir yanıt alma oranı) %1'den az olmalı
    errors: ['rate<0.01'],
  },
};

export default function () {
  // AuthLogService varsayılan olarak 5064 portunda çalışıyor
  const url = 'http://localhost:5089/api/auth/login';

  // Brute-Force simülasyonu için rastgele kullanıcı adı ve şifre üretiyoruz
  const payload = JSON.stringify({
    Username: `user${Math.floor(Math.random() * 10000)}`,
    Password: generateRandomPassword(),
  });

  const params = { headers: { 'Content-Type': 'application/json' } };

  const res = http.post(url, payload, params);

  const checkOutput = check(res, {
    'Durum kodu 401 (Unauthorized)': (r) => r.status === 401,
  });

  errorRate.add(!checkOutput);
}