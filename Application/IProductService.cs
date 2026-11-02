using Application.Models;

namespace Application
{
    public interface IProductService
    {
        Guid CreateProduct(ApplicationProductModel product);
        ApplicationProductModel GetById(Guid productId);
        List<ApplicationProductModel> GetFilteredProducts(Guid? id,
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
                                                      int? pageSize);
    }
}