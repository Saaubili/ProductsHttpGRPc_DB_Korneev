namespace Infrastructure
{
    public class ProductsStorage : IProductsStorage
    {
        private readonly List<InfrastructureProduct> _products = new();

        public Guid Add(InfrastructureProduct product)
        {
            var newProduct = new InfrastructureProduct();
            newProduct.ProductId = Guid.NewGuid();
            newProduct.ProductName = product.ProductName;
            newProduct.ProductPrice = product.ProductPrice;
            newProduct.ProductWeight = product.ProductWeight;
            newProduct.ProductType = product.ProductType;
            newProduct.CreationDate = DateTimeOffset.Now;
            newProduct.WarehouseNumber = product.WarehouseNumber;
            _products.Add(newProduct);
            return newProduct.ProductId;

        }

        public InfrastructureProduct? GetById(Guid id)
        {
            foreach (var product in _products)
            {
                if (product.ProductId == id)
                    return product;
            }
            return null;
        }

        public List<InfrastructureProduct> GetFilteredProducts(Guid? id,
                                                        string? productName,
                                                        float? priceFrom,
                                                        float? priceTo,
                                                        float? weightFrom,
                                                        float? weightTo,
                                                        InfrastructureProductTypes? type,
                                                        int? warehouseNumber,
                                                        DateTimeOffset? createdFrom,
                                                        DateTimeOffset? createdTo,
                                                        int? pageNumber,
                                                        int? pageSize)
        {
            IEnumerable<InfrastructureProduct> result = _products;

            if (id.HasValue)
                result = result.Where(p => p.ProductId == id.Value);

            if (!string.IsNullOrWhiteSpace(productName))
                result = result.Where(p => p.ProductName.Contains(productName));

            if (priceFrom.HasValue)
                result = result.Where(p => p.ProductPrice >= priceFrom.Value);

            if (priceTo.HasValue)
                result = result.Where(p => p.ProductPrice <= priceTo.Value);

            if (weightFrom.HasValue)
                result = result.Where(p => p.ProductWeight >= weightFrom.Value);

            if (weightTo.HasValue)
                result = result.Where(p => p.ProductWeight <= weightTo.Value);

            if (type.HasValue)
                result = result.Where(p => p.ProductType == type.Value);

            if (warehouseNumber.HasValue)
                result = result.Where(p => p.WarehouseNumber == warehouseNumber.Value);

            if (createdFrom.HasValue)
                result = result.Where(p => p.CreationDate >= createdFrom.Value);

            if (createdTo.HasValue)
                result = result.Where(p => p.CreationDate <= createdTo.Value);

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                result = result
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return result.ToList();
        }
    }
}
