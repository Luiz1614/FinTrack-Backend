using Fintrack.Contracts.Pagination;
using FinTrack.Domain.Entities;

namespace FinTrack.Infraestructure.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<bool> DeleteTransactionAsync(int id);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync(TransactionParameters transactionParameters);
        Task<IEnumerable<Transaction>> GetByAccountAsync(int accountId);
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetTransactionByMonthAsync(int idUser, int year, int month);
        Task<Transaction?> UpdateTransactionAsync(Transaction transaction);
    }
}