using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankBlazorAPI.BankModels;

namespace BankBlazorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly BankBlazorContext _context;

        public CustomersController(BankBlazorContext context)
        {
            _context = context;
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetCustomersPaginated(int page = 1, int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            var totalCount = await _context.Customers.CountAsync();

            var customers = await _context.Customers
                .OrderBy(c => c.CustomerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    customerId = c.CustomerId,
                    firstName = c.Givenname,
                    lastName = c.Surname,
                    emailAddress = c.Emailaddress
                })
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalCount,
                items = customers
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == id)
                .Select(c => new
                {
                    customerId = c.CustomerId,
                    firstName = c.Givenname,
                    lastName = c.Surname,
                    emailAddress = c.Emailaddress,
                    addresses = new[]
                    {
                        new
                        {
                            addressLine1 = c.Streetaddress,
                            city = c.City,
                            countryRegion = c.Country
                        }
                    },
                    accounts = c.Dispositions
                        .Select(d => new
                        {
                            accountId = d.Account.AccountId,
                            balance = d.Account.Balance
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }
    }
}