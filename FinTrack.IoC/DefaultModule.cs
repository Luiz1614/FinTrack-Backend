using FinTrack.Infraestructure.Data.Context;
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
    }
}
