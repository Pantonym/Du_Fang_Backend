using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Du_Fang;

namespace Du_Fang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AppDBContext _context;

        public TransactionController(AppDBContext context)
        {
            _context = context;
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

            if (transaction == null)
            {
                return NotFound();
            }

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

        // POST: api/Transaction/AccountTransfer
        [HttpPost("AccountTransfer")]
        public async Task<ActionResult<Transaction>> AccountTransfer(int fromAccountId, int toAccountId, decimal amount)
        {
            // Validate accounts, check balance, and perform transfer logic
            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = amount,
                TransactionType = "AccountTransfer",
                Timestamp = DateTime.UtcNow

                // TODO: Update the balance of the account (foreign key to the account, based on to and from accounts)
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }

        // POST: api/Transaction/AccountTopup
        [HttpPost("AccountTopup")]
        public async Task<ActionResult<Transaction>> AccountTopup(int fromAccountId, decimal amount)
        {
            // Validate account, check balance, and perform purchase logic
            // --Validation
            var account = await _context.Accounts.FindAsync(fromAccountId);

            if (account == null)
            {
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

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }

        // POST: api/Transaction/StarCoinPurchase
        [HttpPost("StarCoinPurchase")]
        public async Task<ActionResult<Transaction>> StarCoinPurchase(int fromAccountId, int starCoinAmount, decimal amount)
        {
            // Validate account, check balance, and perform purchase logic
            // --Validation
            var account = await _context.Accounts.FindAsync(fromAccountId);

            if (account == null)
            {
                return NotFound("Account does not exist.");
            }

            // Update account and StarCoin balance
            account.Balance += amount;
            account.CoinBalance += starCoinAmount;

            // --Build the transaction
            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = fromAccountId, //the user essentially transfers one form of money to another for themselves, and as such they are both receiver and sender.
                Amount = amount,
                TransactionType = "StarCoinPurchase",
                Timestamp = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

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