using DroneTrackingAPI.Models;

namespace DroneTrackingAPI.Services
{
    // BackgroundService, .NET'in arka planda sürekli çalışan işlemler için kullandığı harika bir yapıdır.
    public class DroneSimulationService : BackgroundService
    {
        private readonly ILogger<DroneSimulationService> _logger;

        public DroneSimulationService(ILogger<DroneSimulationService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Drone Simülasyon motoru başlatıldı! Dronelar havalanıyor...");
            Random rnd = new Random();

            // Program kapanana kadar sonsuz döngüde çalışacak
            while (!stoppingToken.IsCancellationRequested)
            {
                // Rastgele sahte drone verisi üretiyoruz
                var telemetry = new DroneTelemetry
                {
                    DroneId = "DRONE-001",
                    Latitude = 39.920770 + (rnd.NextDouble() * 0.02 - 0.01), // Ankara merkezli rastgele enlem
                    Longitude = 32.854110 + (rnd.NextDouble() * 0.02 - 0.01), // Ankara merkezli rastgele boylam
                    Altitude = rnd.Next(100, 500),
                    BatteryLevel = rnd.Next(10, 100),
                    Status = "Uçuyor",
                    Timestamp = DateTime.UtcNow
                };

                // İleride bu veriyi AWS Kinesis'e fırlatacağız ama şimdilik çalıştığını görmek için terminale yazdırıyoruz
                _logger.LogInformation($"[CANLI VERİ] ID: {telemetry.DroneId} | Konum: {telemetry.Latitude:F5}, {telemetry.Longitude:F5} | İrtifa: {telemetry.Altitude}m | Batarya: %{telemetry.BatteryLevel}");

                // Motoru 1 saniye uyut (Saniyede 1 veri akışı için)
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}