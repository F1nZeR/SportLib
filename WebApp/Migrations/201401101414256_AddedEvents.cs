namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEvents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Count = c.Int(nullable: false),
                        Comment = c.String(),
                        TimePeriod = c.Int(nullable: false),
                        Time = c.Time(nullable: false, precision: 7),
                        GameParticipantPlayerId = c.Int(nullable: false),
                        EventTypeId = c.Int(nullable: false),
                        ParentEventId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventTypes", t => t.EventTypeId, cascadeDelete: true)
                .ForeignKey("dbo.GameParticipantPlayers", t => t.GameParticipantPlayerId, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.ParentEventId)
                .Index(t => t.EventTypeId)
                .Index(t => t.GameParticipantPlayerId)
                .Index(t => t.ParentEventId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "ParentEventId", "dbo.Events");
            DropForeignKey("dbo.Events", "GameParticipantPlayerId", "dbo.GameParticipantPlayers");
            DropForeignKey("dbo.Events", "EventTypeId", "dbo.EventTypes");
            DropIndex("dbo.Events", new[] { "ParentEventId" });
            DropIndex("dbo.Events", new[] { "GameParticipantPlayerId" });
            DropIndex("dbo.Events", new[] { "EventTypeId" });
            DropTable("dbo.Events");
        }
    }
}
