using Fintrack.Contracts.DTOs.Account;
using FinTrack.Application.DTOs.Accounts;

namespace FinTrack.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountWithTransactionDto> AddAccountAsync(AccountCreateDto accountCreateDto);
        Task<bool> DeleteAccountAsync(int idAccount, int idUser);
        Task<IEnumerable<AccountDto>> GetAllAccountsAsync(int idUser);
        Task<AccountWithTransactionDto> GetAccountByIdAsync(int idAccount, int idUser);
        Task<IEnumerable<AccountWithTransactionDto>> GetAllAccountsWithTransactionsAsync(int idUser);
        Task<AccountWithTransactionDto> UpdateAccountAsync(AccountUpdateDto accountDto);
    }
}