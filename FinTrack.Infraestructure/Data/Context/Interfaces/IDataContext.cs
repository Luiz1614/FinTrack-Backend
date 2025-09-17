using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using FinTrack.Domain.Entities;

namespace FinTrack.Infraestructure.Data.Context.Interfaces
{
    public interface IDataContext
    {
        DbSet<Account> Accounts { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Transaction> Transactions { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}