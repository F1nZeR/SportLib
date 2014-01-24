namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedsportProps : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EventTypes", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Sports", "TeamSizeMin", c => c.Int());
            AlterColumn("dbo.Sports", "TeamSizeMax", c => c.Int());
            AlterColumn("dbo.PlayerPropertyTypes", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.TeamPropertyTypes", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TeamPropertyTypes", "Name", c => c.String());
            AlterColumn("dbo.PlayerPropertyTypes", "Name", c => c.String());
            AlterColumn("dbo.Sports", "TeamSizeMax", c => c.Int(nullable: false));
            AlterColumn("dbo.Sports", "TeamSizeMin", c => c.Int(nullable: false));
            AlterColumn("dbo.EventTypes", "Name", c => c.String());
        }
    }
}
