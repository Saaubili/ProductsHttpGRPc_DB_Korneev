using Application;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace Presentation.Interceptors
{
    public class ExceptionInterceptor : Interceptor
    {
        private readonly ILogger<ExceptionInterceptor> _logger;

        public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (ValidationException ex)
            {
                var errors = new List<string>();
                foreach (var error in ex.Errors)
                {
                    errors.Add($"{error.PropertyName}: {error.ErrorMessage}");
                }
                throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join("; ", errors)));
            }
            catch (ProductNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанная ошибка gRPC");
                throw new RpcException(new Status(StatusCode.Internal, "Внутренняя ошибка сервера"));
            }
        }
    }
}