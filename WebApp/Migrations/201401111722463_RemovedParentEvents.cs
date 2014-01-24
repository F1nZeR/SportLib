namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedParentEvents : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EventTypes", "ParentEventTypeId", "dbo.EventTypes");
            DropForeignKey("dbo.Events", "ParentEventId", "dbo.Events");
            DropIndex("dbo.EventTypes", new[] { "ParentEventTypeId" });
            DropIndex("dbo.Events", new[] { "ParentEventId" });
            AddColumn("dbo.Events", "GameId", c => c.Int(nullable: false));
            AlterColumn("dbo.Events", "TimePeriod", c => c.Int());
            AlterColumn("dbo.Events", "Time", c => c.Time(precision: 7));
            CreateIndex("dbo.Events", "GameId");
            AddForeignKey("dbo.Events", "GameId", "dbo.Games", "Id", cascadeDelete: false);
            DropColumn("dbo.Events", "ParentEventId");
            DropColumn("dbo.EventTypes", "ParentEventTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EventTypes", "ParentEventTypeId", c => c.Int());
            AddColumn("dbo.Events", "ParentEventId", c => c.Int());
            DropForeignKey("dbo.Events", "GameId", "dbo.Games");
            DropIndex("dbo.Events", new[] { "GameId" });
            AlterColumn("dbo.Events", "Time", c => c.Time(nullable: false, precision: 7));
            AlterColumn("dbo.Events", "TimePeriod", c => c.Int(nullable: false));
            DropColumn("dbo.Events", "GameId");
            CreateIndex("dbo.Events", "ParentEventId");
            CreateIndex("dbo.EventTypes", "ParentEventTypeId");
            AddForeignKey("dbo.Events", "ParentEventId", "dbo.Events", "Id");
            AddForeignKey("dbo.EventTypes", "ParentEventTypeId", "dbo.EventTypes", "Id");
        }
    }
}
