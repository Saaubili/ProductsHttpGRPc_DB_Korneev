using Application.Models;
using Domain;
namespace Application.Mappers
{
    public static class ApplicationToDomainMapper
    {
        public static DomainProduct MapApplicationlToDomain(ApplicationProductModel applicationModel)
        {
            var product = new DomainProduct();
            product.ProductId = applicationModel.ProductId;
            product.ProductName = applicationModel.ProductName;
            product.ProductPrice = applicationModel.ProductPrice;
            product.ProductWeight = applicationModel.ProductWeight;
            product.ProductType = Enum.Parse<DomainProductTypes>(applicationModel.ProductType.ToString());
            product.CreationDate = applicationModel.CreationDate;
            product.WarehouseNumber = applicationModel.WarehouseNumber;
            return product;
        }

        public static ApplicationProductModel MapDomainToApplication(DomainProduct product)
        {
            var applicationModel = new ApplicationProductModel();
            applicationModel.ProductId = product.ProductId;
            applicationModel.ProductName = product.ProductName;
            applicationModel.ProductPrice = product.ProductPrice;
            applicationModel.ProductWeight = product.ProductWeight;
            applicationModel.ProductType = Enum.Parse<ApplicationProductTypes>(product.ProductType.ToString());
            applicationModel.CreationDate = product.CreationDate;
            applicationModel.WarehouseNumber = product.WarehouseNumber;
            return applicationModel;
        }
    }
}
