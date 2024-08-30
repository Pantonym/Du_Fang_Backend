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
    public class AccountController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IAccountService _accountService;

        public AccountController(AppDBContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        // GET: api/Account
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        // GET: api/Account/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Account/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.AccountId)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // POST: api/Account
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.AccountId }, account);
        }

        // DELETE: api/Account/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }

        [HttpPut("{id}/freeze")]
        public async Task<IActionResult> FreezeAccount(int id)
        {
            try
            {
                // Log the received ID
                Console.WriteLine($"Received ID: {id}");

                // Fetch the account with the provided ID
                var account = await _context.Accounts.FindAsync(id);

                // Log the retrieved account details
                if (account == null)
                {
                    Console.WriteLine($"Account with ID {id} not found.");
                    return NotFound($"Account with ID {id} not found.");
                }

                Console.WriteLine($"Account before freezing: {account}");

                // Implement the logic to freeze the account here
                account.Active = false; // Example property to indicate the account is frozen

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Log success message
                Console.WriteLine($"Account with ID {id} has been successfully frozen.");

                return Ok("Account frozen successfully.");
            }
            catch (Exception ex)
            {
                // Log error details
                Console.WriteLine($"Error freezing account with ID {id}: {ex.Message}");
                return StatusCode(500, $"An error occurred while freezing the account with ID {id}: {ex.Message}");
            }
        }

        // POST: api/Account/{id}/unfreeze
        [HttpPut("{id}/unfreeze")]
        public async Task<IActionResult> UnfreezeAccount(int id)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);
                if (account == null)
                {
                    return NotFound();
                }

                await _accountService.UnfreezeAccount(account);
                return Ok("Account unfrozen successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}

public interface IAccountService
{
    Task FreezeAccount(Account account);
    Task FreezeAccount(Du_Fang.Account account);
    Task UnfreezeAccount(Account account);
    Task UnfreezeAccount(Du_Fang.Account account);
}

public class AccountService : IAccountService
{
    private readonly AppDBContext _context;

    public AccountService(AppDBContext context)
    {
        _context = context;
    }

    public async Task FreezeAccount(Account account)
    {
        if (account == null) throw new ArgumentNullException(nameof(account));
        account.Active = false; // Set status as frozen
        await _context.SaveChangesAsync();
    }

    public Task FreezeAccount(Du_Fang.Account account)
    {
        throw new NotImplementedException();
    }

    public async Task UnfreezeAccount(Account account)
    {
        if (account == null) throw new ArgumentNullException(nameof(account));
        account.Active = true; // Set status as active
        await _context.SaveChangesAsync();
    }

    public Task UnfreezeAccount(Du_Fang.Account account)
    {
        throw new NotImplementedException();
    }
}

// Account Model

public class Account
{
    public int AccountId { get; set; }
    public bool Active { get; set; }
}

public enum AccountStatus
{
    Active,
    Frozen
}