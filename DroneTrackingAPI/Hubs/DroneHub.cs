using Microsoft.AspNetCore.SignalR;

namespace DroneTrackingAPI.Hubs
{
    // Hub sınıfından miras alarak buranın bir SignalR merkezi olduğunu belirtiyoruz.
    public class DroneHub : Hub
    {
        // İstemciler (Dashboard), bu sınıf üzerinden açılan kanal üzerinden
        // sunucuyla çift yönlü (Bidirectional) bağlantı kurar.
    }
}