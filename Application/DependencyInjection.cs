using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IProductService, ProductService>();
        services.AddInfrastructure(connectionString);
        return services;
    }
}
