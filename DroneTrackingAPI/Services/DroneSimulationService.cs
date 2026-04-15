using DroneTrackingAPI.Hubs;
using DroneTrackingAPI.Models;
using Microsoft.AspNetCore.SignalR; // SignalR kütüphanesi eklendi

namespace DroneTrackingAPI.Services
{
    public class DroneSimulationService : BackgroundService
    {
        private readonly ILogger<DroneSimulationService> _logger;
        private readonly IHubContext<DroneHub> _hubContext; // Yayın yapmamızı sağlayacak araç

        // Constructor (Yapıcı Metot) güncellendi
        public DroneSimulationService(ILogger<DroneSimulationService> logger, IHubContext<DroneHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("6 Kargo Drone'luk Filo havalanıyor! Gerçekçi uçuş başladı...");
            Random rnd = new Random();

            // 1. AŞAMA: 6 Adet Drone'u uçuşa hazırla (Sadece bir kere oluşturulur)
            var droneFleet = new List<DroneTelemetry>();
            for (int i = 1; i <= 6; i++)
            {
                droneFleet.Add(new DroneTelemetry
                {
                    DroneId = $"DRONE-00{i}",
                    Latitude = 39.920770 + (rnd.NextDouble() * 0.04 - 0.02),
                    Longitude = 32.854110 + (rnd.NextDouble() * 0.04 - 0.02),
                    Altitude = rnd.Next(100, 300),
                    BatteryLevel = rnd.Next(80, 100), // Uçuşa dolu pille başlarlar
                    Status = "Görevde",
                    Timestamp = DateTime.UtcNow
                });
            }

            // 2. AŞAMA: Havada tut ve verileri yavaş yavaş değiştir (Sonsuz Döngü)
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var drone in droneFleet)
                {
                    // Gerçekçi Uçuş: Koordinatları çok minik hareket ettir
                    drone.Latitude += (rnd.NextDouble() * 0.0002 - 0.0001);
                    drone.Longitude += (rnd.NextDouble() * 0.0002 - 0.0001);

                    // İrtifa rüzgara göre 1-2 metre değişsin
                    drone.Altitude += rnd.Next(-2, 3);

                    // Gerçekçi Batarya: Batarya asla artmaz, her saniye %10 ihtimalle 1 puan düşer
                    if (rnd.Next(0, 10) > 8 && drone.BatteryLevel > 0)
                    {
                        drone.BatteryLevel -= 1;
                    }

                    // Eğer pili bitmek üzereyse durumunu güncelle
                    if (drone.BatteryLevel <= 20) drone.Status = "Dönüşe Geçti";
                    if (drone.BatteryLevel == 0) drone.Status = "İndi";

                    drone.Timestamp = DateTime.UtcNow;

                    // Güncel halini React'e fırlat
                    await _hubContext.Clients.All.SendAsync("ReceiveDroneData", drone, stoppingToken);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}