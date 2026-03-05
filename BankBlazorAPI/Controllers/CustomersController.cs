using Microsoft.AspNetCore.Http;
using BankBlazorAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BankBlazorAPI.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BankBlazorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AdventureWorksLt2022Context _context;

        public CustomersController(AdventureWorksLt2022Context context)
        {
            _context = context;
        }

        [HttpGet("paginated")]
        public IActionResult GetCustomersPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 200) pageSize = 200;

            var total = _context.Customers.Count();

            var items = _context.Customers
                .OrderBy(c => c.CustomerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<Customer>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetCustomerById(int id)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == id)
                .Select(c => new
                {
                    c.CustomerId,
                    c.FirstName,
                    c.LastName,
                    c.EmailAddress,
                    Addresses = c.CustomerAddresses
                        .Select(ca => ca.Address)
                        .Select(a => new
                        {
                            a.AddressLine1,
                            a.City,
                            a.CountryRegion
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (customer is null)
                return NotFound();

            return Ok(customer);
        }
    }
}