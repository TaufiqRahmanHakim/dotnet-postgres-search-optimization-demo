using CustomerDashboard.DTO;
using CustomerDashboard.Service.Interface;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion;

namespace CustomerDashboard.Service {


    public class CacheService : ICacheService {
        //private readonly IDistributedCache _cache;
        private readonly IFusionCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IFusionCache cache, ILogger<CacheService> logger) {
            _cache = cache;
            _logger = logger;
        }

        //public async Task<List<CustomerDto>> GetCustomerCache(string key) {
        //    byte[] bytes = await _cache.GetAsync(key);

        //    if (bytes == null) return null;
        //    //_logger.LogInformation("GetCustomerCache {key} Success", key);

        //    return MessagePackSerializer.Deserialize<List<CustomerDto>>(bytes);

        //    //string? cacheCustomer = _cache.GetString(key);
        //    //if (!string.IsNullOrEmpty(cacheCustomer)) {
        //    //    //_logger.LogInformation("GetCustomerCache {key} Success", key);
        //    //    return JsonSerializer.Deserialize<List<CustomerDto>>(cacheCustomer);
        //    //} else {
        //    //    //_logger.LogInformation("GetCustomerCache {key} Null", key);
        //    //    return null;
        //    //}
        //}
        public async Task<List<CustomerDto>> GetCustomerCacheOrFetch(string key, Func<Task<List<CustomerDto>>> factory) {
            // FusionCache otomatis: 
            // 1. Cek RAM (Cepat!)
            // 2. Kalau ga ada, Cek Redis (MessagePack)
            // 3. Kalau ga ada, Jalankan fungsi factory (Ambil dari DB)
            // 4. Simpan ke Redis & RAM

            var data = await _cache.GetOrSetAsync<List<CustomerDto>>(
                key,
                _ => factory(), 
                tags: ["customer-data"]
            );

            return data;
        }
        public async Task<List<CustomerDto>> GetCustomerCache(string key) {
            // Dengan FusionCache, kita jarang pakai Get/Set manual terpisah.
            // Kita pakai pola GetOrSetAsync (seperti di atas).
            // Tapi jika memaksa manual get:
            return await _cache.GetOrDefaultAsync<List<CustomerDto>>(key);
        }

        public async Task SetCustomerCache(List<CustomerDto> data, string key) {
            await _cache.SetAsync(key, data);
        }

        //public async Task SetCustomerCache(List<CustomerDto> data, string key) {
        //    byte[] bytes = MessagePackSerializer.Serialize(data);
        //    var cacheOption = new DistributedCacheEntryOptions {
        //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10), // Cache kadaluarsa dalam 10 menit
        //        SlidingExpiration = TimeSpan.FromMinutes(2)
        //    };
        //    //_logger.LogInformation("SetCustomerCache {key} Success", key);
        //    await _cache.SetAsync(key, bytes, cacheOption);
        //    //string jsonData = JsonSerializer.Serialize(data);
        //    ////_logger.LogInformation("SetCustomerCache {key} Success", key);
        //    //await _cache.SetStringAsync(key, jsonData, cacheOption);
        //}
    }
}

