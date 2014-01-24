namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedRequiredAttributes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sports", "Description", c => c.String());
            AlterColumn("dbo.Games", "Place", c => c.String());
            AlterColumn("dbo.PlayerProperties", "Value", c => c.String());
            AlterColumn("dbo.TeamProperties", "Value", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TeamProperties", "Value", c => c.String(nullable: false));
            AlterColumn("dbo.PlayerProperties", "Value", c => c.String(nullable: false));
            AlterColumn("dbo.Games", "Place", c => c.String(nullable: false));
            AlterColumn("dbo.Sports", "Description", c => c.String(nullable: false));
        }
    }
}
