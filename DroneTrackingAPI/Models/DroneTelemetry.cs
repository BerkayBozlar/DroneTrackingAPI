namespace DroneTrackingAPI.Models
{
    public class DroneTelemetry
    {
        // Her drone'un benzersiz kimliği (Örn: DRONE-001)
        public string DroneId { get; set; }

        // Konum verileri
        public double Latitude { get; set; }  // Enlem
        public double Longitude { get; set; } // Boylam
        public double Altitude { get; set; }  // İrtifa (Yükseklik)

        // Drone'un durumu
        public int BatteryLevel { get; set; } // % Batarya
        public string Status { get; set; }    // "Uçuyor", "Teslim Ediyor", "Dönüyor"

        // Verinin üretildiği an (Gerçek zamanlı akışın kalbi)
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}