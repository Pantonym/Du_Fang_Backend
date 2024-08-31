using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Du_Fang;
using Du_Fang.Services;

namespace Du_Fang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly AccountService _accountService;

        public TransactionController(AppDBContext context, AccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        // GET: api/Transaction
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        // GET: api/Transaction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null) return NotFound();

            return transaction;
        }

        // PUT: api/Transaction/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Transfers from one account to another in the local economy.
        /// </summary>
        /// <param name="fromAccountId"></param>
        /// <param name="toAccountId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        // POST: api/Transaction/AccountTransfer
        [HttpPost("AccountTransfer")]
        public async Task<ActionResult<Transaction>> AccountTransfer(int fromAccountId, int toAccountId, decimal amount)
        {
            // Find the fromAccount and toAccount
            var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
            var toAccount = await _context.Accounts.FindAsync(toAccountId);

            // Validation
            if (fromAccount == null || toAccount == null)
            {
                return NotFound("One or both accounts do not exist.");
            }

            // Validate the fromAccount has enough money
            if (fromAccount.Balance < amount)
            {
                return BadRequest("Insufficient balance in the sender's account.");
            }

            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine($"<color=green>Before Transfer - FromAccount Balance: {fromAccount.Balance}, ToAccount Balance: {toAccount.Balance}</color>");

            // Perform balance updates
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine($"After Transfer - FromAccount Balance: {fromAccount.Balance}, ToAccount Balance: {toAccount.Balance}");

            // Mark both accounts as modified
            _context.Entry(fromAccount).State = EntityState.Modified;
            _context.Entry(toAccount).State = EntityState.Modified;

            // Validate accounts, check balance, and perform transfer logic
            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = amount,
                TransactionType = "AccountTransfer",
                Timestamp = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Log before upgrading status
            Console.WriteLine("Transaction saved, attempting to upgrade status.");

            // Check and upgrade status
            await _accountService.CheckAndUpgradeStatus(fromAccountId);

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }

        // POST: api/Transaction/AccountTopup
        [HttpPost("AccountTopup")]
        public async Task<ActionResult<Transaction>> AccountTopup(int fromAccountId, decimal amount)
        {
            // Log the incoming request
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine($"AccountTopup called with fromAccountId: {fromAccountId}, amount: {amount}");

            // Validate account, check balance, and perform purchase logic
            // --Validation
            var account = await _context.Accounts.FindAsync(fromAccountId);

            if (account == null)
            {
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                Console.WriteLine("Account not found.");
                return NotFound("Account does not exist.");
            }

            // --Update the account balance
            account.Balance += amount;

            // --Build transaction
            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = fromAccountId, //the user essentially transfers one form of money to another for themselves, and as such they are both receiver and sender.
                Amount = amount,
                TransactionType = "AccountTopup",
                Timestamp = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Log before upgrading status
            Console.WriteLine("Transaction saved, attempting to upgrade status.");

            // Check and upgrade status
            await _accountService.CheckAndUpgradeStatus(fromAccountId);

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }

        // POST: api/Transaction/AccountWithdraw
        [HttpPost("AccountWithdraw")]
        public async Task<ActionResult<Transaction>> AccountWithdraw(int fromAccountId, decimal amount)
        {
            // Log the incoming request
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine($"AccountWithdraw called with fromAccountId: {fromAccountId}, amount: {amount}");

            // Validate account, check balance, and perform purchase logic
            // --Validation
            var account = await _context.Accounts.FindAsync(fromAccountId);

            if (account == null)
            {
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                Console.WriteLine("Account not found.");
                return NotFound("Account does not exist.");
            }

            // --Update the account balance
            account.Balance -= amount;

            // --Build transaction
            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = fromAccountId, //the user essentially transfers one form of money to another for themselves, and as such they are both receiver and sender.
                Amount = amount,
                TransactionType = "AccountWithdraw",
                Timestamp = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Log before upgrading status
            Console.WriteLine("Transaction saved, attempting to upgrade status.");

            // Check and upgrade status
            await _accountService.CheckAndUpgradeStatus(fromAccountId);

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }

        // POST: api/Transaction/StarCoinPurchase
        [HttpPost("StarCoinPurchase")]
        public async Task<ActionResult<Transaction>> StarCoinPurchase(int fromAccountId, int starCoins, decimal amount)
        {
            // Validate account, check balance, and perform purchase logic
            // --Validation
            var account = await _context.Accounts.FindAsync(fromAccountId);

            if (account == null)
            {
                return NotFound("Account does not exist.");
            }

            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine($"Star Coin Change called with fromAccountId: {fromAccountId}, amount: {amount}, Starcoins: {starCoins}");

            // Update account and StarCoin balance
            account.Balance += amount;
            account.CoinBalance += starCoins;

            // Determine transaction type based on whether coins are being bought or sold
            string transType;
            if (starCoins < 0)
            {
                transType = "StarcoinSell";
            }
            else
            {
                transType = "StarCoinPurchase";
            }

            // --Build the transaction
            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = fromAccountId, //the user essentially transfers one form of money to another for themselves, and as such they are both receiver and sender.
                Amount = amount,
                TransactionType = transType,
                Timestamp = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Log before upgrading status
            Console.WriteLine("Transaction saved, attempting to upgrade status.");

            // Check and upgrade status
            await _accountService.CheckAndUpgradeStatus(fromAccountId);

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }

        // DELETE: api/Transaction/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }
    }
}