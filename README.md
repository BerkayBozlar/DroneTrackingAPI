# Endüstriyel IoT Drone Takip Sistemi & Büyük Veri Yönetim Paneli

**DroneTrackingAPI**, endüstriyel kargo dronelarından gelen anlık telemetri verilerini (konum, batarya, irtifa) eş zamanlı olarak işlemek, kalıcı olarak depolamak ve bulut tabanlı büyük veri (Big Data) hatlarına aktarmak amacıyla geliştirilmiş uçtan uca (Full-Stack) bir IoT çözümüdür.

🎥 **[Proje Tanıtım ve Mimari Anlatım Videosunu İzlemek İçin Tıklayın](https://drive.google.com/file/d/15jDabFmxR5Php1RFCO4Ec85yVUppTG6M/view?usp=drive_link)**

## Proje Amacı
Bu proje, saniyede yüzlerce veri üreten IoT cihazlarının yarattığı yoğun trafiği yönetebilecek yazılım mühendisliği süreçlerinin uçtan uca (Frontend, Backend, NoSQL Veritabanı ve Bulut Dağıtımı) nasıl entegre edilebileceğini kanıtlamak amacıyla geliştirilmiştir. Kullanıcılar, bir drone filosunun anlık hareketlerini dinamik bir arayüz üzerinden anlık olarak takip edebilir ve bu verileri AWS bulutunda ve MongoDB'de saklayabilirler.

## Kullanılan Teknolojiler ve Mimari

Sistem, birbirinden bağımsız çalışan ancak eş zamanlı veri akış standartlarıyla haberleşen üç ana katmandan oluşmaktadır:

### 1. Backend (Sunucu ve IoT Simülasyon Katmanı)
* **Framework:** C# .NET 8.0 (ASP.NET Core Web API)
* **Real-Time İletişim:** SignalR (WebSocket tabanlı anlık veri transferi)
* **Simülasyon Motoru:** `BackgroundService` ile yönetilen durumsal (stateful) drone simülasyonu.
* **Özellikler:** Asenkron veri işleme boru hattı (Data Pipeline), HTTP uç noktaları, gerçekçi koordinat ve batarya tüketim algoritmaları.

### 2. Frontend (Gerçek Zamanlı Dashboard Katmanı)
* **Kütüphane:** React.js
* **Mimari:** JSX, Fonksiyonel Bileşenler (Functional Components), React Hooks (useState, useEffect) ve CSS Grid.
* **Özellikler:** API'den ve WebSocket üzerinden anlık veri çekme, "Sıfır Yenileme" (Zero Refresh) deneyimi, dinamik batarya indikatörleri, modern ve duyarlı (responsive) UI/UX tasarımı.

### 3. Veritabanı ve Bulut Mimarisi (Kalıcı Depolama & Big Data)
Proje yerel ortamdan çıkarılarak AWS bulut servisleri ve NoSQL mimarisiyle entegre edilmiştir.
* **NoSQL Veritabanı (MongoDB):** Telemetri verilerini JSON formatında kalıcı olarak saklayan, doküman tabanlı hızlı yazma ve geçmişe dönük veri analitiği sağlayan yüksek performanslı katman.
* **AWS Kinesis Data Streams:** Büyük veri (Big Data) ölçeklendirmesi için kullanılan veri akış servisi. Sharding ve Partition Key (DroneId) kullanımıyla bulut tabanlı yük dengelemesi (Load Balancing) sağlanmıştır.

## Temel Özellikler
* **Anlık Telemetri Takibi:** 6 adet drone'un enlem, boylam, irtifa ve batarya bilgilerini canlı izleme.
* **Gerçekçi Simülasyon:** Droneların uçuş süresine bağlı olarak azalan batarya seviyeleri ve milimetrik koordinat değişimleri.
* **Çok Kanallı Veri Dağıtımı:** Üretilen verinin aynı anda arayüze, MongoDB'ye ve AWS bulutuna asenkron olarak fırlatılması.
* **Siber Güvenlik:** IAM rolleri ve geçici erişim anahtarları ile yönetilen güvenli bulut bağlantısı.
