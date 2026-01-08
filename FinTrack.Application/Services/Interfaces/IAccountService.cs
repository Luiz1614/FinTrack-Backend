using Fintrack.Contracts.DTOs.Account;
using FinTrack.Application.DTOs.Accounts;

namespace FinTrack.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDto> AddAccountAsync(AccountCreateDto accountCreateDto);
        Task<bool> DeleteAccountAsync(int idAccount, int idUser);
        Task<AccountDto> GetAccountByIdAsync(int idAccount, int idUser);
        Task<IEnumerable<AccountDto>> GetAllAccountsAsync(int idUser);
        Task<AccountDto> UpdateAccountAsync(AccountUpdateDto accountDto);
    }
}