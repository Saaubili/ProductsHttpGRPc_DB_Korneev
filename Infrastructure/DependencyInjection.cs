using FluentMigrator.Runner;
using Infrastructure;
using Infrastructure.Migrations;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IProductsStorage> (new ProductsStorage(connectionString));
        services.AddFluentMigratorCore()
                .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(CreateProductsTable).Assembly)
                .For.Migrations());

        return services;
    }
}