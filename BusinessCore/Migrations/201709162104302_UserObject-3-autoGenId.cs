namespace BusinessCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserObject3autoGenId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StoreCategories", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Products", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.StoreCategories", "Store_Id", "dbo.Stores");
            DropPrimaryKey("dbo.Categories");
            DropPrimaryKey("dbo.Stores");
            DropPrimaryKey("dbo.Products");
            DropPrimaryKey("dbo.Users");
            AlterColumn("dbo.Categories", "Id", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.Stores", "Id", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.Products", "Id", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.Users", "Id", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.Categories", "Id");
            AddPrimaryKey("dbo.Stores", "Id");
            AddPrimaryKey("dbo.Products", "Id");
            AddPrimaryKey("dbo.Users", new[] { "Id", "Name" });
            AddForeignKey("dbo.StoreCategories", "Category_Id", "dbo.Categories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Products", "Category_Id", "dbo.Categories", "Id");
            AddForeignKey("dbo.StoreCategories", "Store_Id", "dbo.Stores", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreCategories", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Products", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.StoreCategories", "Category_Id", "dbo.Categories");
            DropPrimaryKey("dbo.Users");
            DropPrimaryKey("dbo.Products");
            DropPrimaryKey("dbo.Stores");
            DropPrimaryKey("dbo.Categories");
            AlterColumn("dbo.Users", "Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.Products", "Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.Stores", "Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.Categories", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Users", new[] { "Id", "Name" });
            AddPrimaryKey("dbo.Products", "Id");
            AddPrimaryKey("dbo.Stores", "Id");
            AddPrimaryKey("dbo.Categories", "Id");
            AddForeignKey("dbo.StoreCategories", "Store_Id", "dbo.Stores", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Products", "Category_Id", "dbo.Categories", "Id");
            AddForeignKey("dbo.StoreCategories", "Category_Id", "dbo.Categories", "Id", cascadeDelete: true);
        }
    }
}
