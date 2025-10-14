using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Data.Context.Interfaces;
using FinTrack.Infraestructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infraestructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IDataContext _context;

    public TransactionRepository(IDataContext context)
    {
        _context = context;
    }

    public async Task<Transaction> AddTransactionAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Account)
            .FirstAsync(t => t.Id == transaction.Id);
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        var entity = await _context.Transactions.FindAsync(id);
        if (entity == null)
            return false;

        _context.Transactions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Account)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByAccountAsync(int accountId)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Account)
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int id)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction?> UpdateTransactionAsync(Transaction transaction)
    {
        var entity = await _context.Transactions.FindAsync(transaction.Id);
        if (entity == null)
            return null;

        entity.Title = transaction.Title;
        entity.Type = transaction.Type;
        entity.Amount = transaction.Amount;
        entity.CategoryId = transaction.CategoryId;
        entity.AccountId = transaction.AccountId;

        _context.Transactions.Update(entity);
        await _context.SaveChangesAsync();

        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == entity.Id);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionByMonthAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);

        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Account)
            .Where(t => t.CreatedAt >= start && t.CreatedAt <= end)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}