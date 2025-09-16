using Fintrack.Contracts.DTOs.Account;
using FinTrack.Application.DTOs.Accounts;

namespace FinTrack.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDto> AddAccountAsync(AccountCreateDto accountCreateDto);
        Task<bool> DeleteAccountAsync(int id);
        Task<AccountDto> GetAccountByIdAsync(int id);
        Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
        Task<AccountDto> UpdateAccountAsync(AccountUpdateDto accountDto);
    }
}