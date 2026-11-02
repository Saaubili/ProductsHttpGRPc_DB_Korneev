using Application.Models;

namespace Presentation.Models
{
    public record ProductsRequest (Guid? id,
                                   string? productName,
                                   float? priceFrom,
                                   float? priceTo,
                                   float? weightFrom,
                                   float? weightTo,
                                   ApplicationProductTypes? type,
                                   int? warehouseNumber,
                                   DateTimeOffset? createdFrom,
                                   DateTimeOffset? createdTo,
                                   int? pageNumber = 1,
                                   int? pageSize = 10);
}
