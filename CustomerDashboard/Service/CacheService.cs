using CustomerDashboard.DTO;
using CustomerDashboard.Service.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CustomerDashboard.Service {


    public class CacheService : ICacheService {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger) {
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<CustomerDto>> GetCustomerCache(string key) {
            string? cacheCustomer = _cache.GetString(key);
            if (!string.IsNullOrEmpty(cacheCustomer)) {
                //_logger.LogInformation("GetCustomerCache {key} Success", key);
                return JsonSerializer.Deserialize<List<CustomerDto>>(cacheCustomer);
            } else {
                //_logger.LogInformation("GetCustomerCache {key} Null", key);
                return null;
            }
        }

        public async Task SetCustomerCache(List<CustomerDto> data, string key) {
            var cacheOption = new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10), // Cache kadaluarsa dalam 10 menit
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            string jsonData = JsonSerializer.Serialize(data);
            //_logger.LogInformation("SetCustomerCache {key} Success", key);
            await _cache.SetStringAsync(key, jsonData, cacheOption);
        }
    }
}
