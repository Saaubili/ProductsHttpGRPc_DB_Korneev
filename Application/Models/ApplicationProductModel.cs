namespace Application.Models
{
    public class ApplicationProductModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public float ProductPrice { get; set; }
        public float ProductWeight { get; set; }
        public ApplicationProductTypes ProductType { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public int WarehouseNumber { get; set; }
    }
}
