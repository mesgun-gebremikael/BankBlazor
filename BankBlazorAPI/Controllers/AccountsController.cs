using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankBlazorAPI.BankModels;

namespace BankBlazorAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly BankBlazorContext _context;

        public AccountsController(BankBlazorContext context)
        {
            _context = context;
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerAccounts(int customerId)
        {
            var accounts = await _context.Dispositions
                .Where(d => d.CustomerId == customerId)
                .Select(d => new
                {
                    d.Account.AccountId,
                    d.Account.Balance
                })
                .ToListAsync();

            return Ok(accounts);
        }
    }
}