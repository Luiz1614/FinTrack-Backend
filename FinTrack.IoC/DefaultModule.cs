using FinTrack.Application.Services;
using FinTrack.Application.Services.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Data.Context;
using FinTrack.Infraestructure.Data.Context.Interfaces;
using FinTrack.Infraestructure.Repositories;
using FinTrack.Infraestructure.Repositories.Interfaces;
using FinTrack.Transform.Profiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinTrack.IoC;

public class DefaultModule
{
    public static void Start(IServiceCollection service, IConfiguration configuration)
    {
        service.AddDbContext<DataContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("MySql");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        service.AddScoped<IDataContext>(sp => sp.GetRequiredService<DataContext>());

        service.AddTransient<ICategoryRepository, CategoryRepository>();
        service.AddTransient<ICategoryService, CategoryService>();

        service.AddTransient<IAccountRepository, AccountRepository>();
        service.AddTransient<IAccountService, AccountService>();

        service.AddTransient<ITransactionRepository, TransactionRepository>();
        service.AddTransient<ITransactionService, TransactionService>();

        service.AddTransient<IReportService, ReportService>();

        service.AddAutoMapper( cfg => { },
            typeof(AccountProfile),
            typeof(CategoryProfile),
            typeof(TransactionProfile));
    }
}