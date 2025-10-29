using System.Threading;
using System.Threading.Tasks;
using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Data.Context.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infraestructure.Data.Context;

public class DataContext : IdentityDbContext<Users>, IDataContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);

    public int SaveChanges()
        => base.SaveChanges();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}