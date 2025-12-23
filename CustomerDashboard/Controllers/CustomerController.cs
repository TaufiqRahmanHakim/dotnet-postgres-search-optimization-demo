using CustomerDashboard.Data;
using CustomerDashboard.DTO;
using CustomerDashboard.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CustomerDashboard.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase {

        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ICacheService _cacheService;

        public CustomerController(ApplicationDbContext context, IDistributedCache cache, ICacheService cacheService) {
            _context = context;
            _cache = cache;
            _cacheService = cacheService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10) {

            string key = $"customer_GetCustomersBySearch_{search}_{page}_{pageSize}";
            var cacheCustomer = await _cacheService.GetCustomerCache(key);
            if (cacheCustomer != null) {
                return Ok(cacheCustomer);
            }

            var query = _context.Customers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search)) {
                var terms = search.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (terms.Length > 0) {
                    var firstWord = terms[0];

                    query = query.Where(c =>
                        c.FirstName.ToLower().StartsWith(firstWord) ||
                        c.LastName.ToLower().StartsWith(firstWord) ||
                        c.City.ToLower().StartsWith(firstWord)
                    );

                    if (terms.Length > 1) {
                        foreach (var term in terms.Skip(1)) {
                            var currentTerm = term;

                            query = query.Where(c =>
                                c.FirstName.ToLower().Contains(currentTerm) ||
                                c.LastName.ToLower().Contains(currentTerm) ||
                                c.City.ToLower().Contains(currentTerm)
                            );
                        }
                    }
                }
            }

            #region not optimal for specific
            //if (!string.IsNullOrWhiteSpace(search)) {
            //    var searchLower = search.ToLower();

            //    //not optimized
            //    //query = query.Where(c =>
            //    //    EF.Functions.ILike(c.FirstName, $"{search}%") ||
            //    //    EF.Functions.ILike(c.LastName, $"{search}%") ||
            //    //    EF.Functions.ILike(c.City, $"{search}%"));

            //    query = query.Where(c => 
            //        c.FirstName.ToLower().StartsWith(searchLower) || 
            //        c.LastName.ToLower().StartsWith(searchLower) ||
            //        c.City.ToLower().StartsWith(searchLower));
            //}
            #endregion
            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderBy(c => c.Id) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerDto {
                    Id = c.Id,
                    FullName = $"{c.FirstName} {c.LastName}",
                    Email = c.Email,
                    City = c.City,
                    Status = c.IsActive ? "Active" : "Inactive"
                })
                .ToListAsync();

            await _cacheService.SetCustomerCache(data, key);

            return Ok(new {
                //TotalData = totalRecords,
                //Page = page,
                //Size = pageSize,
                Data = data
            });
        }
        [HttpGet("fast-paging")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomersFast(
            [FromQuery] int lastSeenId = 0,
            [FromQuery] int limit = 20) {

            #region Redis Get and Check
            string key = $"customer_fast-paging_{lastSeenId}";
            var customer = await _cacheService.GetCustomerCache(key);
            if(customer != null) {
                return Ok(customer);
            }

            #endregion 

            var data = await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Where(c => c.Id > lastSeenId) 
                .Take(limit)
                .Select(c => new CustomerDto {
                    Id = c.Id,
                    FullName = $"{c.FirstName} {c.LastName}",
                    Email = c.Email,
                    City = c.City,
                    Status = c.IsActive ? "Active" : "Inactive"
                })
                .ToListAsync();

            #region Set Redis
            await _cacheService.SetCustomerCache(data, key);
            #endregion

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id) {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null) {
                return NotFound();
            }
            return new CustomerDto {
                Id = customer.Id,
                FullName = $"{customer.FirstName} {customer.LastName}",
                Email = customer.Email,
                City = customer.City,
                Status = customer.IsActive ? "Active" : "Inactive"
            };
        }

        [HttpPost("bulk-deactivate")]
        public async Task<IActionResult> BulkDeactivate() {
            var affectedRows = await _context.Customers
                .Where(c => c.LoyaltyPoints == 0 && c.IsActive)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsActive, false));

            return Ok(new { Message = $"Berhasil menonaktifkan {affectedRows} user." });
        }
    }
}
