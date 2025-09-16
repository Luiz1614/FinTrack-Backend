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
        return transaction;
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        var entity = await GetTransactionByIdAsync(id);
        if (entity == null)
            return false;

        _context.Transactions.Remove(entity);
        return true;
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await _context.Transactions
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByAccountAsync(int accountId)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int id)
    {
        return await _context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction?> UpdateTransactionAsync(Transaction transaction)
    {
        var entity = await GetTransactionByIdAsync(transaction.Id);

        _context.Transactions.Update(entity);

        return entity;
    }
}