using FinTrack.Domain.Entities;

namespace FinTrack.Infraestructure.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> AddAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(int idAccount, int idUser);
        Task<Account?> GetAccountByIdAsync(int idAccount, int idUser);
        Task<IEnumerable<Account>> GetAllAccountsAsync(int idUser);
        Task<Account> UpdateAccountAsync(Account account);
        Task<Account?> GetAccountWithTransactionsAsync(int id, int idUser);
        Task<IEnumerable<Account>> GetAllAccountsWithTransactionsAsync(int idUser);
    }
}