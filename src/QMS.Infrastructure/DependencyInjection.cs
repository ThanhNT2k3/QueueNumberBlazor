using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QMS.Domain.Interfaces;
using QMS.Infrastructure.Persistence;
using QMS.Infrastructure.Persistence.Repositories;

namespace QMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Default to SQLite if no connection string or if configured
        // In a real scenario, you might check a provider config
        services.AddDbContext<QmsDbContext>(options =>
            options.UseSqlite(connectionString ?? "Data Source=qms.db"));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
