namespace BusinessCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(maxLength: 150),
                        Description = c.String(maxLength: 1000),
                        CreationDate = c.DateTime(nullable: false),
                        CreationUser = c.String(),
                        ModificationDate = c.DateTime(nullable: false),
                        ModificatioUser = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Coordinates = c.String(),
                        ImageURL = c.String(maxLength: 2083),
                        ThumbnailURL = c.String(maxLength: 2083),
                        Name = c.String(maxLength: 150),
                        Description = c.String(maxLength: 1000),
                        CreationDate = c.DateTime(nullable: false),
                        CreationUser = c.String(),
                        ModificationDate = c.DateTime(nullable: false),
                        ModificatioUser = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImageURL = c.String(maxLength: 2083),
                        ThumbnailURL = c.String(maxLength: 2083),
                        Name = c.String(maxLength: 150),
                        Description = c.String(maxLength: 1000),
                        CreationDate = c.DateTime(nullable: false),
                        CreationUser = c.String(),
                        ModificationDate = c.DateTime(nullable: false),
                        ModificatioUser = c.String(),
                        Category_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128),
                        Password = c.String(maxLength: 10),
                        Level = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        CreationUser = c.String(),
                        ModificationDate = c.DateTime(nullable: false),
                        ModificatioUser = c.String(),
                    })
                .PrimaryKey(t => new { t.Id, t.Name });
            
            CreateTable(
                "dbo.StoreCategories",
                c => new
                    {
                        Store_Id = c.Guid(nullable: false),
                        Category_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Store_Id, t.Category_Id })
                .ForeignKey("dbo.Stores", t => t.Store_Id, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.Category_Id, cascadeDelete: true)
                .Index(t => t.Store_Id)
                .Index(t => t.Category_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.StoreCategories", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.StoreCategories", "Store_Id", "dbo.Stores");
            DropIndex("dbo.StoreCategories", new[] { "Category_Id" });
            DropIndex("dbo.StoreCategories", new[] { "Store_Id" });
            DropIndex("dbo.Products", new[] { "Category_Id" });
            DropTable("dbo.StoreCategories");
            DropTable("dbo.Users");
            DropTable("dbo.Products");
            DropTable("dbo.Stores");
            DropTable("dbo.Categories");
        }
    }
}
