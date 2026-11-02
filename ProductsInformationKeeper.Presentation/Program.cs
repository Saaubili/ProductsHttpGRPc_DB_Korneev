using Application;
using FluentValidation;
using Presentation.Controllers;
using Presentation.Interceptors;
using Presentation.Middleware;
using Presentation.Services;
using Presentation.Validators;
using System.Text.Json.Serialization;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplication();
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