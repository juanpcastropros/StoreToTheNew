namespace BusinessCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserObject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Password = c.String(maxLength: 10),
                        Level = c.Int(nullable: false),
                        Id = c.Guid(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        CreationUser = c.String(),
                        ModificationDate = c.DateTime(nullable: false),
                        ModificatioUser = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
            AddColumn("dbo.Categories", "CreationUser", c => c.String());
            AddColumn("dbo.Categories", "ModificatioUser", c => c.String());
            AddColumn("dbo.Stores", "CreationUser", c => c.String());
            AddColumn("dbo.Stores", "ModificatioUser", c => c.String());
            AddColumn("dbo.Products", "CreationUser", c => c.String());
            AddColumn("dbo.Products", "ModificatioUser", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ModificatioUser");
            DropColumn("dbo.Products", "CreationUser");
            DropColumn("dbo.Stores", "ModificatioUser");
            DropColumn("dbo.Stores", "CreationUser");
            DropColumn("dbo.Categories", "ModificatioUser");
            DropColumn("dbo.Categories", "CreationUser");
            DropTable("dbo.Users");
        }
    }
}
