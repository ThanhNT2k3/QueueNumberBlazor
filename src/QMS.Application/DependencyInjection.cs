using Microsoft.Extensions.DependencyInjection;
using QMS.Application.Interfaces;
using QMS.Application.Services;

namespace QMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<ICounterService, CounterService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}
