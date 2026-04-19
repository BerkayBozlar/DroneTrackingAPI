import { useState, useEffect } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import './App.css';

function App() {
  const [drones, setDrones] = useState({});
  const [connectionStatus, setConnectionStatus] = useState('Bağlanıyor...');

  useEffect(() => {

    const backendUrl = "http://localhost:5025/droneHub"; // KENDİ PORTUNLA DEĞİŞTİR

    const newConnection = new HubConnectionBuilder()
      .withUrl(backendUrl)
      .withAutomaticReconnect()
      .build();

    newConnection.start()
      .then(() => {
        setConnectionStatus('Canlı Bağlantı Aktif 🟢');
        
        // C#'tan fırlattığımız "ReceiveDroneData" isimli yayını dinliyoruz
        newConnection.on("ReceiveDroneData", (drone) => {
          setDrones(prevDrones => ({
            ...prevDrones,
            [drone.droneId]: drone // Gelen drone verisini ID'sine göre günceller
          }));
        });
      })
      .catch(err => {
        console.error("SignalR Bağlantı Hatası: ", err);
        setConnectionStatus('Bağlantı Koptu 🔴');
      });

    return () => {
      newConnection.stop();
    };
  }, []);

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <h1>Kargo Drone Takip Merkezi</h1>
        <span className="status-badge">{connectionStatus}</span>
      </header>

      <div className="drone-grid">
        {Object.values(drones).length === 0 ? (
          <p className="loading-text">Drone verileri bekleniyor...</p>
        ) : (
          Object.values(drones).map((drone) => (
            <div key={drone.droneId} className="drone-card active">
              <div className="card-header">
                <h2>{drone.droneId}</h2>
                <span className="status-indicator">{drone.status}</span>
              </div>
              
              <div className="card-body">
                <div className="data-row">
                  <span className="label">Enlem:</span>
                  <span className="value">{drone.latitude.toFixed(5)}</span>
                </div>
                <div className="data-row">
                  <span className="label">Boylam:</span>
                  <span className="value">{drone.longitude.toFixed(5)}</span>
                </div>
                <div className="data-row">
                  <span className="label">İrtifa:</span>
                  <span className="value">{drone.altitude} m</span>
                </div>
                
                <div className="battery-container">
                  <span className="label">Batarya: %{drone.batteryLevel}</span>
                  <div className="battery-bar-bg">
                    <div 
                      className={`battery-bar-fill ${drone.batteryLevel <= 20 ? 'danger' : ''}`} 
                      style={{ width: `${drone.batteryLevel}%` }}
                    ></div>
                  </div>
                </div>
              </div>
              <div className="timestamp">
                Son Güncelleme: {new Date(drone.timestamp).toLocaleTimeString()}
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}

export default App;