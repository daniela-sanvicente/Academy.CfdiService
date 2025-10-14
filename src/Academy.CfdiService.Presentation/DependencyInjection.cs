using Microsoft.Extensions.DependencyInjection;

namespace Academy.CfdiService.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        return services;
    }
}
