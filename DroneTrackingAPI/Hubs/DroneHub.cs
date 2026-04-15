using Microsoft.AspNetCore.SignalR;

namespace DroneTrackingAPI.Hubs
{
    // Hub sınıfından miras alarak buranın bir SignalR merkezi olduğunu belirtiyoruz.
    public class DroneHub : Hub
    {
        // İleride React'ten C#'a bir komut (Örn: "Drone'u indir") göndermek istersek buraya metodlar yazacağız.
        // Biz şu an sadece C#'tan React'e tek yönlü veri fırlatacağımız için içi boş kalabilir.
    }
}