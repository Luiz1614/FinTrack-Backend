using Fintrack.Contracts.Pagination;
using FinTrack.Domain.Entities;

namespace FinTrack.Infraestructure.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<bool> DeleteTransactionAsync(int idTransaction, int idUser);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync(TransactionParameters transactionParameters, int idUser);
        Task<IEnumerable<Transaction>> GetByAccountAsync(int idAccount, int idUser);
        Task<Transaction?> GetTransactionByIdAsync(int idAccount, int idUser);
        Task<IEnumerable<Transaction>> GetTransactionByMonthAsync(int idUser, int year, int month);
        Task<Transaction?> UpdateTransactionAsync(Transaction transaction, int idUser);
    }
}