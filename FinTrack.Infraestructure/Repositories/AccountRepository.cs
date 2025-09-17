using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Data.Context.Interfaces;
using FinTrack.Infraestructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infraestructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IDataContext _context;

    public AccountRepository(IDataContext context)
    {
        _context = context;
    }

    public async Task<Account> AddAccountAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        var accountEntity = await GetAccountByIdAsync(id);
        if (accountEntity == null)
            return false;

        _context.Accounts.Remove(accountEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        return await _context.Accounts.AsNoTracking().ToListAsync();
    }

    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> UpdateAccountAsync(Account account)
    {
        var exists = await _context.Accounts.AsNoTracking().AnyAsync(a => a.Id == account.Id);
        if (!exists)
            throw new KeyNotFoundException($"Account with id {account.Id} not found.");

        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<Account?> GetAccountWithTransactionsAsync(int id)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Include(a => a.Transactions)
                .ThenInclude(t => t.Category)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Account>> GetAllAccountsWithTransactionsAsync()
    {
        return await _context.Accounts
            .AsNoTracking()
            .Include(a => a.Transactions)
                .ThenInclude(t => t.Category)
            .ToListAsync();
    }
}