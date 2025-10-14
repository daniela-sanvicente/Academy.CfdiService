using Academy.CfdiService.Domain.Repositories;
using Academy.CfdiService.Infrastructure.Persistence;
using Academy.CfdiService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Academy.CfdiService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CfdiDatabase")
            ?? throw new InvalidOperationException("Connection string 'CfdiDatabase' was not found.");

        services.AddDbContext<CfdiDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICfdiReadRepository, EfCfdiReadRepository>();

        return services;
    }
}
