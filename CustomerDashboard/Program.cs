

using CustomerDashboard.Data;
using CustomerDashboard.Service;
using CustomerDashboard.Service.Interface;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.NeueccMessagePack;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))

);
//redis init
var redisConnection = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = redisConnection;
    options.InstanceName = "CustomerInstance_"; 
});
builder.Services.AddScoped<ICacheService, CacheService>();

// Di Program.cs
builder.Services.AddFusionCache()
    .WithDefaultEntryOptions(new FusionCacheEntryOptions {
        Duration = TimeSpan.FromMinutes(10),

        // FAIL-SAFE: Jika DB/Redis error, pakai data lama di memori (biar user ga kena error)
        IsFailSafeEnabled = true,
        FailSafeMaxDuration = TimeSpan.FromHours(2), // Data lama boleh hidup sampai 2 jam jika darurat

        // SOFT TIMEOUT: Jika ambil data baru > 100ms, kasih data lama dulu, lalu update di background
        //FactorySoftTimeout = TimeSpan.FromMilliseconds(100),

        // HARD TIMEOUT: Batas maksimal nunggu DB, cegah loading muter terus
        FactoryHardTimeout = TimeSpan.FromMilliseconds(1500)
    })
    .WithSerializer(new FusionCacheNeueccMessagePackSerializer()) // Serialize super cepat
    .WithRegisteredDistributedCache();
    


builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
});

var app = builder.Build();
app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
