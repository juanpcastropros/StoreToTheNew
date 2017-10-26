namespace BusinessCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserObject2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Users");
            AddPrimaryKey("dbo.Users", new[] { "Id", "Name" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Users");
            AddPrimaryKey("dbo.Users", "Name");
        }
    }
}
