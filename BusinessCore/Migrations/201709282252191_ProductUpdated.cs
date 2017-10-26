namespace BusinessCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductUpdated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductStores", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.ProductStores", "Store_Id", "dbo.Stores");
            DropIndex("dbo.ProductStores", new[] { "Product_Id" });
            DropIndex("dbo.ProductStores", new[] { "Store_Id" });
            AddColumn("dbo.Products", "Store_Id", c => c.Guid());
            CreateIndex("dbo.Products", "Store_Id");
            AddForeignKey("dbo.Products", "Store_Id", "dbo.Stores", "Id");
            DropTable("dbo.ProductStores");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductStores",
                c => new
                    {
                        Product_Id = c.Guid(nullable: false),
                        Store_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Product_Id, t.Store_Id });
            
            DropForeignKey("dbo.Products", "Store_Id", "dbo.Stores");
            DropIndex("dbo.Products", new[] { "Store_Id" });
            DropColumn("dbo.Products", "Store_Id");
            CreateIndex("dbo.ProductStores", "Store_Id");
            CreateIndex("dbo.ProductStores", "Product_Id");
            AddForeignKey("dbo.ProductStores", "Store_Id", "dbo.Stores", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ProductStores", "Product_Id", "dbo.Products", "Id", cascadeDelete: true);
        }
    }
}
