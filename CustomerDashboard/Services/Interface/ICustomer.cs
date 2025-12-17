using CustomerDashboard.DTO;

namespace CustomerDashboard.Services.Interface {
    public interface ICustomer {
        public Task<CustomerDto> GetCustomer();
        
    }
}
