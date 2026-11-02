using System;
namespace Infrastructure;

public class InfrastructureProduct
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public float ProductPrice { get; set; }
    public float ProductWeight { get; set; }
    public InfrastructureProductTypes ProductType { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public int WarehouseNumber { get; set; }
}