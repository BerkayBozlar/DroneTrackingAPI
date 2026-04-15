var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. BÖLÜM: SERVÝS HAZIRLIKLARI (BUILDER)
// ==========================================

builder.Services.AddControllers();

// Swagger (API dökümantasyonu) ayarlarý
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR Servisi
builder.Services.AddSignalR();

// Simülasyon Servisimizi (Arka plan motoru) ekliyoruz
builder.Services.AddHostedService<DroneTrackingAPI.Services.DroneSimulationService>();

// CORS Politikasý (React'in bizimle haberleþmesi için güvenlik duvarýný deliyoruz)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React'in portu
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // SignalR için þart
    });
});

// Tüm hazýrlýklar bitti, projeyi inþa et
var app = builder.Build();

// ==========================================
// 2. BÖLÜM: UYGULAMA KURALLARI (APP)
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// DÝKKAT: CORS, Authorization'dan önce olmak zorundadýr!
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

// SignalR Yayýn Frekansýmýzý (Endpoint) belirliyoruz
app.MapHub<DroneTrackingAPI.Hubs.DroneHub>("/droneHub");

// FÝNÝÞ ÇÝZGÝSÝ: Uygulamayý ayaða kaldýr (Bundan sonra kod yazýlmaz)
app.Run();