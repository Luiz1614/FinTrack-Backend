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

    public async Task<bool> DeleteAccountAsync(int idAccount, int idUser)
    {
        var accountEntity = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == idAccount && x.UserId == idUser);
        if (accountEntity == null)
            return false;

        _context.Accounts.Remove(accountEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync(int idUser)
    {
        return await _context.Accounts.AsNoTracking().Where(a => a.Id == idUser).ToListAsync();
    }

    public async Task<Account> UpdateAccountAsync(Account account)
    {
        var exists = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id);
        if (exists is null)
            throw new KeyNotFoundException($"Account with id {account.Id} not found.");

        exists.Name = account.Name;
        exists.InitialBalance = account.InitialBalance;

        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<Account?> GetAccountByIdAsync(int idAccount, int idUser)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == idAccount && a.UserId == idUser);
    }

    public async Task<Account?> GetAccountWithTransactionsAsync(int id, int idUser)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Include(u => u.User)
            .Include(a => a.Transactions)
            .ThenInclude(t => t.Category)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == idUser);
    }

    public async Task<IEnumerable<Account>> GetAllAccountsWithTransactionsAsync(int idUser)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Include(a => a.Transactions)
                .ThenInclude(t => t.Category)
                .Where(a => a.Id == idUser)
            .ToListAsync();
    }
}