namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemEventAndNullableEventPlayer : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM dbo.EventTypes");
            Sql("DELETE FROM dbo.Events");
            DropForeignKey("dbo.Events", "GameParticipantPlayerId", "dbo.GameParticipantPlayers");
            DropIndex("dbo.Events", new[] { "GameParticipantPlayerId" });
            AddColumn("dbo.EventTypes", "IsSystemEventType", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Events", "GameParticipantPlayerId", c => c.Int());
            CreateIndex("dbo.Events", "GameParticipantPlayerId");
            AddForeignKey("dbo.Events", "GameParticipantPlayerId", "dbo.GameParticipantPlayers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "GameParticipantPlayerId", "dbo.GameParticipantPlayers");
            DropIndex("dbo.Events", new[] { "GameParticipantPlayerId" });
            AlterColumn("dbo.Events", "GameParticipantPlayerId", c => c.Int(nullable: false));
            DropColumn("dbo.EventTypes", "IsSystemEventType");
            CreateIndex("dbo.Events", "GameParticipantPlayerId");
            AddForeignKey("dbo.Events", "GameParticipantPlayerId", "dbo.GameParticipantPlayers", "Id", cascadeDelete: true);
        }
    }
}
