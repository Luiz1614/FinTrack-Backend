using Fintrack.Contracts.DTOs.Transaction;

namespace FinTrack.Application.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> AddTransactionAsync(TransactionCreateDto transactionCreateDto);
        Task<bool> DeleteTransactionAsync(int id);
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
        Task<TransactionDto> GetTransactionByAccountAsync(int accountId);
        Task<TransactionDto> GetTransactionByIdAsync(int id);
        Task<TransactionDto> UpdateTransactionAsync(TransactionUpdateDto transactionUpdateDto);
        Task<IEnumerable<TransactionDto>> GetTrasactionsByMonthAsync(int idUser, int year,  int month);
    }
}