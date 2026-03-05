using Microsoft.AspNetCore.Http;
using BankBlazorAPI.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public IActionResult GetCustomers()
        {
            var customers = _context.Customers.Take(20).ToList();
            return Ok(customers);
        }
    }
}