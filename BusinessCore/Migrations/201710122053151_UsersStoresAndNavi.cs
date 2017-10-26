namespace BusinessCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersStoresAndNavi : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserNavigations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        CreationUser = c.String(),
                        ModificationDate = c.DateTime(nullable: false),
                        ModificatioUser = c.String(),
                        User_Id = c.Guid(),
                        User_Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => new { t.User_Id, t.User_Name })
                .Index(t => new { t.User_Id, t.User_Name });
            
            CreateTable(
                "dbo.UserStores",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        CreationUser = c.String(),
                        ModificationDate = c.DateTime(nullable: false),
                        ModificatioUser = c.String(),
                        Store_Id = c.Guid(),
                        User_Id = c.Guid(),
                        User_Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .ForeignKey("dbo.Users", t => new { t.User_Id, t.User_Name })
                .Index(t => t.Store_Id)
                .Index(t => new { t.User_Id, t.User_Name });
            
            AddColumn("dbo.Categories", "UserNavigation_Id", c => c.Guid());
            CreateIndex("dbo.Categories", "UserNavigation_Id");
            AddForeignKey("dbo.Categories", "UserNavigation_Id", "dbo.UserNavigations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserStores", new[] { "User_Id", "User_Name" }, "dbo.Users");
            DropForeignKey("dbo.UserStores", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.UserNavigations", new[] { "User_Id", "User_Name" }, "dbo.Users");
            DropForeignKey("dbo.Categories", "UserNavigation_Id", "dbo.UserNavigations");
            DropIndex("dbo.UserStores", new[] { "User_Id", "User_Name" });
            DropIndex("dbo.UserStores", new[] { "Store_Id" });
            DropIndex("dbo.UserNavigations", new[] { "User_Id", "User_Name" });
            DropIndex("dbo.Categories", new[] { "UserNavigation_Id" });
            DropColumn("dbo.Categories", "UserNavigation_Id");
            DropTable("dbo.UserStores");
            DropTable("dbo.UserNavigations");
        }
    }
}
