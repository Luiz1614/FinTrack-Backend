using FinTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infraestructure.Data.Context.Interfaces
{
    public interface IDataContext
    {
        DbSet<Account> Accounts { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Transaction> Transactions { get; set; }
    }
}