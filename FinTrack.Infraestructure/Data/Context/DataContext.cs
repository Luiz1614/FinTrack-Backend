using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Data.Context.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infraestructure.Data.Context;

public class DataContext : DbContext, IDataContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
}
