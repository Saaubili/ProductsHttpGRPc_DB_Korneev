
using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(202611170001)]
public class CreateProductsTable : Migration
{
    public override void Up()
    {
        Create.Table("products")
            .WithColumn("product_id").AsGuid().PrimaryKey()
            .WithColumn("product_name").AsString(200).NotNullable()
            .WithColumn("product_price").AsFloat().NotNullable()
            .WithColumn("product_weight").AsFloat().NotNullable()
            .WithColumn("product_type").AsInt32().NotNullable()
            .WithColumn("creation_date").AsDateTimeOffset().NotNullable()
            .WithColumn("warehouse_number").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("products");
    }
}
