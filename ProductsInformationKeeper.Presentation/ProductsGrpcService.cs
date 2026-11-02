using Application;
using Application.Models;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Presentation.Models;
using Presentation.Protos;

namespace Presentation.Services
{
    public class ProductsGrpcService : Products.ProductsBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<ApplicationProductModel> _productValidator;
        private readonly IValidator<ProductsRequest> _requestValidator;

        public ProductsGrpcService(IProductService productService,
                                   IValidator<ApplicationProductModel> productValidator,
                                   IValidator<ProductsRequest> requestValidator)
        {
            _productService = productService;
            _productValidator = productValidator;
            _requestValidator = requestValidator;
        }

        public override Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            var product = new ApplicationProductModel
            {
                ProductName = request.Name,
                ProductPrice = request.Price,
                ProductWeight = request.Weight,
                ProductType = (ApplicationProductTypes)(int)request.Type,
                WarehouseNumber = request.WarehouseNumber
            };

            _productValidator.ValidateAndThrow(product);

            var id = _productService.CreateProduct(product);

            var response = new CreateProductResponse { Id = id.ToString() };
            return Task.FromResult(response);
        }

        public override Task<ProductMessage> GetProductById(GetProductByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный id товара"));
            }

            var product = _productService.GetById(id);
            return Task.FromResult(ToMessage(product));
        }

        public override Task<GetProductsResponse> GetProducts(GetProductsRequest request, ServerCallContext context)
        {
            Guid? id = null;
            if (request.HasId)
            {
                if (!Guid.TryParse(request.Id, out var parsedId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный id товара"));
                }
                id = parsedId;
            }

            ApplicationProductTypes? type = null;
            if (request.HasType)
            {
                type = (ApplicationProductTypes)(int)request.Type;
            }

            var filter = new ProductsRequest(
                id: id,
                productName: request.HasName ? request.Name : null,
                priceFrom: request.HasPriceFrom ? request.PriceFrom : (float?)null,
                priceTo: request.HasPriceTo ? request.PriceTo : (float?)null,
                weightFrom: request.HasWeightFrom ? request.WeightFrom : (float?)null,
                weightTo: request.HasWeightTo ? request.WeightTo : (float?)null,
                type: type,
                warehouseNumber: request.HasWarehouseNumber ? request.WarehouseNumber : (int?)null,
                createdFrom: request.CreatedFrom?.ToDateTimeOffset(),
                createdTo: request.CreatedTo?.ToDateTimeOffset(),
                pageNumber: request.HasPageNumber ? request.PageNumber : 1,
                pageSize: request.HasPageSize ? request.PageSize : 10);

            _requestValidator.ValidateAndThrow(filter);

            var products = _productService.GetFilteredProducts(
                filter.id,
                filter.productName,
                filter.priceFrom,
                filter.priceTo,
                filter.weightFrom,
                filter.weightTo,
                filter.type,
                filter.warehouseNumber,
                filter.createdFrom,
                filter.createdTo,
                filter.pageNumber,
                filter.pageSize);

            var response = new GetProductsResponse();
            foreach (var product in products)
            {
                response.Products.Add(ToMessage(product));
            }

            return Task.FromResult(response);
        }

        private static ProductMessage ToMessage(ApplicationProductModel product)
        {
            return new ProductMessage
            {
                Id = product.ProductId.ToString(),
                Name = product.ProductName,
                Price = product.ProductPrice,
                Weight = product.ProductWeight,
                Type = (ProductType)(int)product.ProductType,
                CreatedAt = Timestamp.FromDateTimeOffset(product.CreationDate),
                WarehouseNumber = product.WarehouseNumber
            };
        }
    }
}