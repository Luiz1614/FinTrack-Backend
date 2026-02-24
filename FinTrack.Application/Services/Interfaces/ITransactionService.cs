using Fintrack.Contracts.DTOs.Transaction;
using Fintrack.Contracts.Pagination;

namespace FinTrack.Application.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> AddTransactionAsync(TransactionCreateDto transactionCreateDto, int idUser);
        Task<bool> DeleteTransactionAsync(int idTransaction, int idUser);
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(TransactionParameters trasactionParameters, int idUser);
        Task<IEnumerable<TransactionDto>> GetTransactionByAccountAsync(int accountId, int idUser);
        Task<TransactionDto> GetTransactionByIdAsync(int idTransaction, int idUser);
        Task<TransactionDto> UpdateTransactionAsync(TransactionUpdateDto transactionUpdateDto, int idUser);
        Task<IEnumerable<TransactionDto>> GetTrasactionsByMonthAsync(int idUser, int year,  int month);
    }
}