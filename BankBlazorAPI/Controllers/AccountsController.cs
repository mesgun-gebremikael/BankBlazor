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

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest("Beloppet måste vara större än 0.");
            }

            if (request.FromAccountId == request.ToAccountId)
            {
                return BadRequest("Du kan inte överföra till samma konto.");
            }

            var fromAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == request.FromAccountId);

            var toAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == request.ToAccountId);

            if (fromAccount == null || toAccount == null)
            {
                return NotFound("Ett eller båda kontona hittades inte.");
            }

            if (fromAccount.Balance < request.Amount)
            {
                return BadRequest("Det finns inte tillräckligt med saldo på frånkontot.");
            }

            fromAccount.Balance -= request.Amount;
            toAccount.Balance += request.Amount;

            var withdrawTransaction = new Transaction
            {
                AccountId = fromAccount.AccountId,
                Date = DateTime.Now,
                Type = "Debit",
                Operation = "Transfer to account " + toAccount.AccountId,
                Amount = request.Amount,
                Balance = fromAccount.Balance,
                Symbol = null,
                Bank = null,
                Account = null
            };

            var depositTransaction = new Transaction
            {
                AccountId = toAccount.AccountId,
                Date = DateTime.Now,
                Type = "Credit",
                Operation = "Transfer from account " + fromAccount.AccountId,
                Amount = request.Amount,
                Balance = toAccount.Balance,
                Symbol = null,
                Bank = null,
                Account = null
            };

            _context.Transactions.Add(withdrawTransaction);
            _context.Transactions.Add(depositTransaction);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Överföring genomförd.",
                fromAccountId = fromAccount.AccountId,
                toAccountId = toAccount.AccountId,
                fromBalance = fromAccount.Balance,
                toBalance = toAccount.Balance
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

    public class TransferRequest
    {
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}