namespace BusinessCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductStores : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StoreCategories", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.StoreCategories", "Category_Id", "dbo.Categories");
            DropIndex("dbo.StoreCategories", new[] { "Store_Id" });
            DropIndex("dbo.StoreCategories", new[] { "Category_Id" });
            CreateTable(
                "dbo.ProductStores",
                c => new
                    {
                        Product_Id = c.Guid(nullable: false),
                        Store_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Product_Id, t.Store_Id })
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.Store_Id, cascadeDelete: true)
                .Index(t => t.Product_Id)
                .Index(t => t.Store_Id);
            
            AddColumn("dbo.Stores", "Category_Id", c => c.Guid());
            CreateIndex("dbo.Stores", "Category_Id");
            AddForeignKey("dbo.Stores", "Category_Id", "dbo.Categories", "Id");
            DropTable("dbo.StoreCategories");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StoreCategories",
                c => new
                    {
                        Store_Id = c.Guid(nullable: false),
                        Category_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Store_Id, t.Category_Id });
            
            DropForeignKey("dbo.Stores", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.ProductStores", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.ProductStores", "Product_Id", "dbo.Products");
            DropIndex("dbo.ProductStores", new[] { "Store_Id" });
            DropIndex("dbo.ProductStores", new[] { "Product_Id" });
            DropIndex("dbo.Stores", new[] { "Category_Id" });
            DropColumn("dbo.Stores", "Category_Id");
            DropTable("dbo.ProductStores");
            CreateIndex("dbo.StoreCategories", "Category_Id");
            CreateIndex("dbo.StoreCategories", "Store_Id");
            AddForeignKey("dbo.StoreCategories", "Category_Id", "dbo.Categories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.StoreCategories", "Store_Id", "dbo.Stores", "Id", cascadeDelete: true);
        }
    }
}
