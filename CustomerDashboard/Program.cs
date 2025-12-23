

using CustomerDashboard.Data;
using CustomerDashboard.Service;
using CustomerDashboard.Service.Interface;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
