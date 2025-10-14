using System;
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
        var provider = configuration["DatabaseOptions:Provider"] ?? "sqlserver";
        var connectionStringName =
            configuration["DatabaseOptions:ConnectionStringName"] ?? "DefaultConnection";

        var connectionString = configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' was not found or is empty.");
        }

        services.AddDbContext<CfdiDbContext>(options =>
        {
            if (string.Equals(provider, "sqlserver", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlServer(connectionString);
                return;
            }

            throw new InvalidOperationException($"Unsupported database provider '{provider}'.");
        });

        services.AddScoped<ICfdiReadRepository, EfCfdiReadRepository>();

        return services;
    }
}
