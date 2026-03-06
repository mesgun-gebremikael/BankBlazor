using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankBlazorAPI.Models;

namespace BankBlazorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AdventureWorksLt2022Context _context;

        public OrdersController(AdventureWorksLt2022Context context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.SalesOrderHeaders
                .Where(o => o.SalesOrderId == id)
                .Select(o => new
                {
                    o.SalesOrderId,
                    o.OrderDate,
                    o.TotalDue
                })
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}