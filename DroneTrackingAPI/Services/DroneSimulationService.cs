using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using System.Text.Json;
using System.Text;
using DroneTrackingAPI.Hubs;
using DroneTrackingAPI.Models;
using Microsoft.AspNetCore.SignalR;
// MONGODB KÜTÜPHANELERİ EKLENDİ
using MongoDB.Driver;
using MongoDB.Bson;

namespace DroneTrackingAPI.Services
{
    public class DroneSimulationService : BackgroundService
    {
        private readonly ILogger<DroneSimulationService> _logger;
        private readonly IHubContext<DroneHub> _hubContext;
        private readonly AmazonKinesisClient _kinesisClient;
        private const string StreamName = "DroneTelemetryStream";

        // MONGODB KOLEKSİYON (TABLO) TANIMLAMASI
        private readonly IMongoCollection<DroneTelemetry> _telemetryCollection;

        public DroneSimulationService(ILogger<DroneSimulationService> logger, IHubContext<DroneHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;

            // AWS Ayarları (Burayı kendi anahtarlarınla dolduracaksın)
            var awsAccessKey = "ACCESS KEY KOY";
            var awsSecretKey = "SecretKey Koy";
            _kinesisClient = new AmazonKinesisClient(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USEast1);

            // MONGODB BAĞLANTISI (Yerel veya Bulut fark etmez, standart bağlantı kodudur)
            var mongoClient = new MongoClient("mongodb://localhost:27017"); // Bilgisayarındaki yerel MongoDB
            var database = mongoClient.GetDatabase("DroneFleetDB"); // Veritabanı adı
            _telemetryCollection = database.GetCollection<DroneTelemetry>("TelemetryHistory"); // Tablo adı
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AWS Kinesis ve MongoDB Bağlantıları Hazır! Filo havalanıyor...");
            Random rnd = new Random();

            var droneFleet = new List<DroneTelemetry>();
            for (int i = 1; i <= 6; i++)
            {
                droneFleet.Add(new DroneTelemetry
                {
                    DroneId = $"DRONE-00{i}",
                    Latitude = 39.920770 + (rnd.NextDouble() * 0.04 - 0.02),
                    Longitude = 32.854110 + (rnd.NextDouble() * 0.04 - 0.02),
                    Altitude = rnd.Next(100, 300),
                    BatteryLevel = rnd.Next(80, 100),
                    Status = "Görevde",
                    Timestamp = DateTime.UtcNow
                });
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var drone in droneFleet)
                {
                    drone.Latitude += (rnd.NextDouble() * 0.0002 - 0.0001);
                    drone.Longitude += (rnd.NextDouble() * 0.0002 - 0.0001);
                    drone.Altitude += rnd.Next(-2, 3);

                    if (rnd.Next(0, 10) > 8 && drone.BatteryLevel > 0)
                        drone.BatteryLevel -= 1;

                    if (drone.BatteryLevel <= 20) drone.Status = "Dönüşe Geçti";
                    if (drone.BatteryLevel == 0) drone.Status = "İndi";

                    drone.Timestamp = DateTime.UtcNow;

                    // 1. REACT EKRANINA GÖNDER (Canlı Yayın)
                    await _hubContext.Clients.All.SendAsync("ReceiveDroneData", drone, stoppingToken);

                    // 2. MONGODB'YE KAYDET (Kalıcı Veritabanı - PDF Şartı)
                    try
                    {
                        await _telemetryCollection.InsertOneAsync(drone, cancellationToken: stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"MongoDB'ye yazılamadı (Kurulu olmayabilir): {ex.Message}");
                    }

                    // 3. AWS KINESIS'E GÖNDER (Büyük Veri Bulutu)
                    try
                    {
                        var jsonData = JsonSerializer.Serialize(drone);
                        var dataBytes = Encoding.UTF8.GetBytes(jsonData);
                        var putRequest = new PutRecordRequest
                        {
                            StreamName = StreamName,
                            Data = new MemoryStream(dataBytes),
                            PartitionKey = drone.DroneId
                        };
                        await _kinesisClient.PutRecordAsync(putRequest, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"AWS'ye gönderilemedi: {ex.Message}");
                    }
                }

                _logger.LogInformation($"[SİSTEM AKTİF] Veriler SignalR, MongoDB ve AWS'ye işlendi.");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}