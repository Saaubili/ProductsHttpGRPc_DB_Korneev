using System.Data;

namespace Infrastructure
{
    public interface IProductsStorage
    {
        Guid Add (InfrastructureProduct product);

        bool Update(InfrastructureProduct product);

        InfrastructureProduct? GetById(Guid id);

        List<InfrastructureProduct>? GetFilteredProducts (Guid? id,
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
                                            int? pageSize);
    }
}
