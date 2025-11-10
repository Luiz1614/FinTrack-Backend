using Fintrack.Contracts.DTOs.Transaction;
using Fintrack.Contracts.Pagination;

namespace FinTrack.Application.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> AddTransactionAsync(TransactionCreateDto transactionCreateDto);
        Task<bool> DeleteTransactionAsync(int id);
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(TransactionParameters trasactionParameters);
        Task<IEnumerable<TransactionDto>> GetTransactionByAccountAsync(int accountId);
        Task<TransactionDto> GetTransactionByIdAsync(int id);
        Task<TransactionDto> UpdateTransactionAsync(TransactionUpdateDto transactionUpdateDto);
        Task<IEnumerable<TransactionDto>> GetTrasactionsByMonthAsync(int idUser, int year,  int month);
    }
}