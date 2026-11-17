using Application.CustomExceptions;
using FluentValidation;

namespace Presentation.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                var errors = new List<string>();
                foreach (var error in ex.Errors)
                {
                    errors.Add($"{error.PropertyName}: {error.ErrorMessage}");
                }
                await WriteErrorAsync(context, StatusCodes.Status400BadRequest, new { errors });
            }
            catch (ProductNotFoundException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status404NotFound, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанная ошибка");
                await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, new { message = "Внутренняя ошибка сервера" });
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, int statusCode, object body)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(body);
        }
    }
}