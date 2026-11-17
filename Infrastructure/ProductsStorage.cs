using Dapper;
using Npgsql;
using System.Text;
namespace Infrastructure
{
    public class ProductsStorage : IProductsStorage
    {
        private readonly string _connectionString;

        public ProductsStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly List<InfrastructureProduct> _products = new();

        public Guid Add(InfrastructureProduct product)
        {
            var newProduct = new InfrastructureProduct
            {
                ProductId = Guid.NewGuid(),
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductWeight = product.ProductWeight,
                ProductType = product.ProductType,
                CreationDate = DateTimeOffset.UtcNow,
                WarehouseNumber = product.WarehouseNumber
            };

            var sqlQuery = """
                            INSERT INTO products
                            (
                                product_id,
                                product_name,
                                product_price,
                                product_weight,
                                product_type,
                                creation_date,
                                warehouse_number
                            )
                            VALUES
                            (
                                @ProductId,
                                @ProductName,
                                @ProductPrice,
                                @ProductWeight,
                                @ProductType,
                                @CreationDate,
                                @WarehouseNumber
                            )
                            RETURNING product_id;
                            """;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            return connection.ExecuteScalar<Guid>(sqlQuery, new
            {
                newProduct.ProductId,
                newProduct.ProductName,
                newProduct.ProductPrice,
                newProduct.ProductWeight,
                ProductType = (int)newProduct.ProductType,
                newProduct.CreationDate,
                newProduct.WarehouseNumber
            });
        }

        public bool Update(InfrastructureProduct product)
        {
            var sqlQuery = """
                            UPDATE products
                            SET
                                product_name = @ProductName,
                                product_price = @ProductPrice,
                                product_weight = @ProductWeight,
                                product_type = @ProductType,
                                warehouse_number = @WarehouseNumber
                            WHERE product_id = @ProductId;
                           """;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var affectedRows = connection.Execute(sqlQuery, new
            {
                product.ProductId,
                product.ProductName,
                product.ProductPrice,
                product.ProductWeight,
                ProductType = (int)product.ProductType,
                product.WarehouseNumber
            });

            return affectedRows > 0;
        }

        public InfrastructureProduct? GetById(Guid id)
        {
            var sqlQuery = """
                            SELECT
                                product_id AS "ProductId",
                                product_name AS "ProductName",
                                product_price AS "ProductPrice",
                                product_weight AS "ProductWeight",
                                product_type AS "ProductType",
                                creation_date AS "CreationDate",
                                warehouse_number AS "WarehouseNumber"
                            FROM products
                            WHERE product_id = @Id;
                           """;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            return connection.QuerySingleOrDefault<InfrastructureProduct>(
                sqlQuery,
                new { Id = id });
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
            var sqlQuery = new StringBuilder("""
                                            SELECT
                                                product_id AS "ProductId",
                                                product_name AS "ProductName",
                                                product_price AS "ProductPrice",
                                                product_weight AS "ProductWeight",
                                                product_type AS "ProductType",
                                                creation_date AS "CreationDate",
                                                warehouse_number AS "WarehouseNumber"
                                            FROM products
                                            """);

            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (id.HasValue)
            {
                conditions.Add("product_id = @Id");
                parameters.Add("Id", id.Value);
            }

            if (!string.IsNullOrWhiteSpace(productName))
            {
                conditions.Add("product_name LIKE '%' || @ProductName || '%'");
                parameters.Add("ProductName", productName);
            }

            if (priceFrom.HasValue)
            {
                conditions.Add("product_price >= @PriceFrom");
                parameters.Add("PriceFrom", priceFrom.Value);
            }

            if (priceTo.HasValue)
            {
                conditions.Add("product_price <= @PriceTo");
                parameters.Add("PriceTo", priceTo.Value);
            }

            if (weightFrom.HasValue)
            {
                conditions.Add("product_weight >= @WeightFrom");
                parameters.Add("WeightFrom", weightFrom.Value);
            }

            if (weightTo.HasValue)
            {
                conditions.Add("product_weight <= @WeightTo");
                parameters.Add("WeightTo", weightTo.Value);
            }

            if (type.HasValue)
            {
                conditions.Add("product_type = @Type");
                parameters.Add("Type", (int)type.Value);
            }

            if (warehouseNumber.HasValue)
            {
                conditions.Add("warehouse_number = @WarehouseNumber");
                parameters.Add("WarehouseNumber", warehouseNumber.Value);
            }

            if (createdFrom.HasValue)
            {
                conditions.Add("creation_date >= @CreatedFrom");
                parameters.Add("CreatedFrom", createdFrom.Value.ToUniversalTime());
            }

            if (createdTo.HasValue)
            {
                conditions.Add("creation_date <= @CreatedTo");
                parameters.Add("CreatedTo", createdTo.Value.ToUniversalTime());
            }

            if (conditions.Count > 0)
            {
                sqlQuery.AppendLine("WHERE " + string.Join(" AND ", conditions));
            }

            sqlQuery.AppendLine("ORDER BY creation_date, product_id");

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                if (pageNumber.Value < 1)
                    throw new ArgumentOutOfRangeException(nameof(pageNumber));

                if (pageSize.Value < 1)
                    throw new ArgumentOutOfRangeException(nameof(pageSize));

                sqlQuery.AppendLine("LIMIT @PageSize OFFSET @Offset");

                parameters.Add("PageSize", pageSize.Value);
                parameters.Add(
                    "Offset",
                    (pageNumber.Value - 1) * pageSize.Value);
            }

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            return connection.Query<InfrastructureProduct>(
                sqlQuery.ToString(),
                parameters).ToList();
        }
    }
}
