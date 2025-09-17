using FinTrack.Domain.Entities;

namespace FinTrack.Infraestructure.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> AddAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(int id);
        Task<Account?> GetAccountByIdAsync(int id);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> UpdateAccountAsync(Account account);
        Task<Account?> GetAccountWithTransactionsAsync(int id);
        Task<IEnumerable<Account>> GetAllAccountsWithTransactionsAsync();
    }
}