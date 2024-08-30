using System;
using Microsoft.EntityFrameworkCore;

namespace Du_Fang.Services;

public class AccountService
{
    private readonly AppDBContext _context;

    public AccountService(AppDBContext context)
    {
        _context = context;
    }

    public async Task CheckAndUpgradeStatus(int accountId)
    {
        var account = await _context.Accounts
            .Include(a => a.TransactionsFrom)
            .FirstOrDefaultAsync(a => a.AccountId == accountId);

        if (account == null)
        {
            throw new Exception("Account not found.");
        }

        // Count the number of transactions initiated by the account - with no transactions in mind
        int transactionCount = account.TransactionsFrom?.Count ?? 0;
        decimal balance = account.Balance;

        // Check and upgrade status based on the criteria
        if (balance >= 50000 || transactionCount >= 100)
        {
            account.StatusId = 4; // Platinum
        }
        else if (balance >= 20000 || transactionCount >= 50)
        {
            account.StatusId = 3; // Gold
        }
        else if (balance >= 5000 || transactionCount >= 10)
        {
            account.StatusId = 2; // Silver
        }
        else if (balance < 5000 || transactionCount < 10)
        {
            account.StatusId = 1; // Bronze
        }

        _context.Entry(account).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task FreezeAccount(Account account)
    {
        if (account == null) throw new ArgumentNullException(nameof(account));
        account.Active = false; // Set status as frozen
        await _context.SaveChangesAsync();
    }

    public async Task UnfreezeAccount(Account account)
    {
        if (account == null) throw new ArgumentNullException(nameof(account));
        account.Active = true; // Set status as active
        await _context.SaveChangesAsync();
    }
}

public interface IAccountService
{
    Task FreezeAccount(Account account);
    Task UnfreezeAccount(Account account);
}