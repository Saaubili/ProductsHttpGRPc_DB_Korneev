using Application.Models;
using Application.CustomExceptions;
using Domain;
using Infrastructure;
using Application.Mappers;

namespace Application
{
    public class ProductService : IProductService
    {
        private readonly IProductsStorage _storage;

        public ProductService(IProductsStorage storage)
        {
            _storage = storage;
        }

        public Guid CreateProduct(ApplicationProductModel product)
        {
            var domainProduct = ApplicationToDomainMapper.MapApplicationlToDomain(product);
            var record = MapDomainToStorage(domainProduct);
            return _storage.Add(record);
        }

        public void UpdateProduct(Guid productId, ApplicationProductModel product)
        {
            var domainProduct = ApplicationToDomainMapper.MapApplicationlToDomain(product);

            domainProduct.ProductId = productId;

            var record = MapDomainToStorage(domainProduct);

            var isUpdated = _storage.Update(record);

            if (!isUpdated)
            {
                throw new ProductNotFoundException(
                    $"Товар с id: {productId} не найден");
            }
        }

        public ApplicationProductModel GetById(Guid productId)
        {
            var record = _storage.GetById(productId);

            if (record == null)
            {
                throw new ProductNotFoundException($"Товар с id: {productId} не найден");
            }

            var domainProduct = MapStorageToDomain(record);
            return ApplicationToDomainMapper.MapDomainToApplication(domainProduct);
        }

        public List<ApplicationProductModel> GetFilteredProducts(Guid? id,
                                                             string? productName,
                                                             float? priceFrom,
                                                             float? priceTo,
                                                             float? weightFrom,
                                                             float? weightTo,
                                                             ApplicationProductTypes? type,
                                                             int? warehouseNumber,
                                                             DateTimeOffset? createdFrom,
                                                             DateTimeOffset? createdTo,
                                                             int? pageNumber,
                                                             int? pageSize)
        {
            InfrastructureProductTypes? storageType = null;
            if (type.HasValue)
            {
                storageType = Enum.Parse<InfrastructureProductTypes>(type.Value.ToString());
            }

            var records = _storage.GetFilteredProducts(id,
                                                       productName,
                                                       priceFrom,
                                                       priceTo,
                                                       weightFrom,
                                                       weightTo,
                                                       storageType,
                                                       warehouseNumber,
                                                       createdFrom,
                                                       createdTo,
                                                       pageNumber,
                                                       pageSize);

            var result = new List<ApplicationProductModel>();

            if (records == null)
            {
                return result;
            }

            foreach (var record in records)
            {
                var domainProduct = MapStorageToDomain(record);
                result.Add(ApplicationToDomainMapper.MapDomainToApplication(domainProduct));
            }

            return result;
        }

        private InfrastructureProduct MapDomainToStorage(DomainProduct product)
        {
            var record = new InfrastructureProduct();
            record.ProductId = product.ProductId;
            record.ProductName = product.ProductName;
            record.ProductPrice = product.ProductPrice;
            record.ProductWeight = product.ProductWeight;
            record.ProductType = Enum.Parse<InfrastructureProductTypes>(product.ProductType.ToString());
            record.CreationDate = product.CreationDate;
            record.WarehouseNumber = product.WarehouseNumber;
            return record;
        }

        private DomainProduct MapStorageToDomain(InfrastructureProduct record)
        {
            var product = new DomainProduct();
            product.ProductId = record.ProductId;
            product.ProductName = record.ProductName;
            product.ProductPrice = record.ProductPrice;
            product.ProductWeight = record.ProductWeight;
            product.ProductType = Enum.Parse<DomainProductTypes>(record.ProductType.ToString());
            product.CreationDate = record.CreationDate;
            product.WarehouseNumber = record.WarehouseNumber;
            return product;
        }
    }
}