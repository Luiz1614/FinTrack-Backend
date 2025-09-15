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
        return account;
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        var category = await GetAccountByIdAsync(id);

        if (category == null)
            return false;

        _context.Accounts.Remove(category);
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
        var entity = await GetAccountByIdAsync(account.Id);

        _context.Accounts.Update(entity);

        return entity;
    }
}
