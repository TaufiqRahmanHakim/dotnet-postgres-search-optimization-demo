using CustomerDashboard.DTO;

namespace CustomerDashboard.Service.Interface {
    public interface ICacheService {
        public Task<List<CustomerDto>> GetCustomerCache(string key);
        public Task SetCustomerCache(List<CustomerDto> data, string key);
        public Task<List<CustomerDto>> GetCustomerCacheOrFetch(string key, Func<Task<List<CustomerDto>>> factory);
    }
}
