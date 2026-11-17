using Application;
using FluentValidation;
using Presentation.Interceptors;
using Presentation.Middleware;
using Presentation.Services;
using Presentation.Validators;
using System.Text.Json.Serialization;
using Application.CustomExceptions;
using FluentMigrator.Runner;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new ConnectionStringNotFoundException("Строка подключения не найдена");

            builder.Services.AddApplication(connectionString);
            builder.Services.AddValidatorsFromAssemblyContaining<ProductsRequestValidator>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddGrpc(options =>
            {
                options.Interceptors.Add<LoggingInterceptor>();
                options.Interceptors.Add<ExceptionInterceptor>();
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                migrationRunner.MigrateUp();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.MapControllers();
            app.MapGrpcService<ProductsGrpcService>();

            app.Run();
        }
    }
}