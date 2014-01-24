namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedImageUrls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sports", "ImageUrl", c => c.String());
            AddColumn("dbo.Participants", "ImageUrl", c => c.String());
            AddColumn("dbo.Tournaments", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "ImageUrl");
            DropColumn("dbo.Participants", "ImageUrl");
            DropColumn("dbo.Sports", "ImageUrl");
        }
    }
}
