using Domain;
using Infrastructure;
namespace Application.Mappers
{
    public static class DomainToInfrastructure
    {
        public static InfrastructureProduct MapDomainToStorage(DomainProduct product)
        {
            var insrastructureRecord = new InfrastructureProduct();
            insrastructureRecord.ProductId = product.ProductId;
            insrastructureRecord.ProductName = product.ProductName;
            insrastructureRecord.ProductPrice = product.ProductPrice;
            insrastructureRecord.ProductWeight = product.ProductWeight;
            insrastructureRecord.ProductType = Enum.Parse<InfrastructureProductTypes>(product.ProductType.ToString());
            insrastructureRecord.CreationDate = product.CreationDate;
            insrastructureRecord.WarehouseNumber = product.WarehouseNumber;
            return insrastructureRecord;
        }

        public static DomainProduct MapStorageToDomain(InfrastructureProduct insrastructureRecord)
        {
            var product = new DomainProduct();
            product.ProductId = insrastructureRecord.ProductId;
            product.ProductName = insrastructureRecord.ProductName;
            product.ProductPrice = insrastructureRecord.ProductPrice;
            product.ProductWeight = insrastructureRecord.ProductWeight;
            product.ProductType = Enum.Parse<DomainProductTypes>(insrastructureRecord.ProductType.ToString());
            product.CreationDate = insrastructureRecord.CreationDate;
            product.WarehouseNumber = insrastructureRecord.WarehouseNumber;
            return product;
        }
    }
}
