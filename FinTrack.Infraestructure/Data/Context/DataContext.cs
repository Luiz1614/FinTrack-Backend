using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Data.Context.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infraestructure.Data.Context;

public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>, IDataContext
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

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<int>>().ToTable("Roles");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

        builder.Entity<User>(entity =>
        {
            entity.Property(u => u.DateOfBirth)
                  .HasColumnType("date");

            entity.Property(u => u.RefreshToken)
                  .HasMaxLength(512)
                  .HasColumnType("varchar(512)");

            entity.Property(u => u.RefreshTokenExpireTime)
                  .HasColumnType("datetime(3)");

            entity.Property(u => u.PasswordHash)
                  .HasMaxLength(256)
                  .HasColumnType("varchar(256)");

            entity.Property(u => u.SecurityStamp)
                  .HasMaxLength(36)
                  .HasColumnType("varchar(36)");

            entity.Property(u => u.ConcurrencyStamp)
                  .HasMaxLength(36)
                  .HasColumnType("varchar(36)");

            entity.Property(u => u.PhoneNumber)
                  .HasMaxLength(20)
                  .HasColumnType("varchar(20)");
        });

        builder.Entity<IdentityRole<int>>(entity =>
        {
            entity.Property(r => r.ConcurrencyStamp)
                  .HasMaxLength(36)
                  .HasColumnType("varchar(36)");
        });

        builder.Entity<IdentityRoleClaim<int>>(entity =>
        {
            entity.Property(rc => rc.ClaimType)
                  .HasMaxLength(256)
                  .HasColumnType("varchar(256)");

            entity.Property(rc => rc.ClaimValue)
                  .HasMaxLength(256)
                  .HasColumnType("varchar(256)");
        });

        builder.Entity<IdentityUserClaim<int>>(entity =>
        {
            entity.Property(uc => uc.ClaimType)
                  .HasMaxLength(256)
                  .HasColumnType("varchar(256)");

            entity.Property(uc => uc.ClaimValue)
                  .HasMaxLength(256)
                  .HasColumnType("varchar(256)");
        });

        builder.Entity<IdentityUserLogin<int>>(entity =>
        {
            entity.Property(ul => ul.ProviderDisplayName)
                  .HasMaxLength(128)
                  .HasColumnType("varchar(128)");
        });

        builder.Entity<IdentityUserToken<int>>(entity =>
        {
            entity.Property(ut => ut.Value)
                  .HasMaxLength(2048)
                  .HasColumnType("varchar(2048)");
        });

        builder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.CreatedAt)
                  .HasColumnType("datetime(3)");
        });
    }
}