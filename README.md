# Microservices Orchestrator - Dispatcher Gateway

Bu proje, mikroservis mimarisi için geliştirilmiş merkezi bir API Gateway çözümüdür. Gelen istekleri karşılar, güvenlik kontrollerini yapar (Rate Limiting) ve trafik verilerini asenkron olarak loglar.

## 🚀 Teknolojiler
* **.NET 9 ASP.NET Core** (Gateway Altyapısı)
* **StackExchange.Redis** (Hızlı Loglama)
* **Grafana** (Veri Görselleştirme ve Dashboard)
* **Docker & Docker Compose** (Konteynerizasyon)

## 🛠️ Kurulum ve Çalıştırma

Proje, 12-Factor App standartlarına uygun olarak tamamen Dockerize edilmiştir. Çalıştırmak için bilgisayarınızda Docker Desktop'ın açık olması yeterlidir.

1. Terminali projenin ana dizininde açın.
2. Aşağıdaki komutu çalıştırarak orkestrasyonu başlatın:
   ```bash
   docker-compose up -d
