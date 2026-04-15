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
            _logger.LogInformation("Drone Simülasyon motoru başlatıldı! Veriler SignalR ile yayına giriyor...");
            Random rnd = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                var telemetry = new DroneTelemetry
                {
                    DroneId = "DRONE-001",
                    Latitude = 39.920770 + (rnd.NextDouble() * 0.02 - 0.01),
                    Longitude = 32.854110 + (rnd.NextDouble() * 0.02 - 0.01),
                    Altitude = rnd.Next(100, 500),
                    BatteryLevel = rnd.Next(10, 100),
                    Status = "Uçuyor",
                    Timestamp = DateTime.UtcNow
                };

                // Eskisi gibi terminale yazdırıyoruz ki arkada çalıştığını görelim
                _logger.LogInformation($"[CANLI YAYIN] Drone-001 | Konum: {telemetry.Latitude:F5}, {telemetry.Longitude:F5}");

                // İŞTE BÜYÜK AN: Veriyi "ReceiveDroneData" adlı kanaldan tüm React kullanıcılarına fırlatıyoruz!
                await _hubContext.Clients.All.SendAsync("ReceiveDroneData", telemetry, stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}