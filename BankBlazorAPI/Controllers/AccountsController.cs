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

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest("Beloppet måste vara större än 0.");
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == request.AccountId);

            if (account == null)
            {
                return NotFound("Kontot hittades inte.");
            }

            account.Balance += request.Amount;

            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Date = DateTime.Now,
                Type = "Credit",
                Operation = "Deposit",
                Amount = request.Amount,
                Balance = account.Balance,
                Symbol = null,
                Bank = null,
                Account = null
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Insättning genomförd.",
                accountId = account.AccountId,
                newBalance = account.Balance
            });
        }
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest("Beloppet måste vara större än 0.");
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == request.AccountId);

            if (account == null)
            {
                return NotFound("Kontot hittades inte.");
            }

            if (account.Balance < request.Amount)
            {
                return BadRequest("Det finns inte tillräckligt med saldo på kontot.");
            }

            account.Balance -= request.Amount;

            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Date = DateTime.Now,
                Type = "Debit",
                Operation = "Withdraw",
                Amount = request.Amount,
                Balance = account.Balance,
                Symbol = null,
                Bank = null,
                Account = null
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Uttag genomfört.",
                accountId = account.AccountId,
                newBalance = account.Balance
            });
        }
    }

    public class DepositRequest
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
    }
    public class WithdrawRequest
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}