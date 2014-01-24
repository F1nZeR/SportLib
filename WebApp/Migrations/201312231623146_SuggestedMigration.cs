namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SuggestedMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GameParticipantPlayers", "GameParticipant_Id", "dbo.GameParticipants");
            DropIndex("dbo.GameParticipantPlayers", new[] { "GameParticipant_Id" });
            RenameColumn(table: "dbo.GameParticipantPlayers", name: "GameParticipant_Id", newName: "GameParticipantId");
            AlterColumn("dbo.GameParticipantPlayers", "GameParticipantId", c => c.Int(nullable: false));
            CreateIndex("dbo.GameParticipantPlayers", "GameParticipantId");
            AddForeignKey("dbo.GameParticipantPlayers", "GameParticipantId", "dbo.GameParticipants", "Id", cascadeDelete: false);
            DropColumn("dbo.GameParticipantPlayers", "GameParticipId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GameParticipantPlayers", "GameParticipId", c => c.Int(nullable: false));
            DropForeignKey("dbo.GameParticipantPlayers", "GameParticipantId", "dbo.GameParticipants");
            DropIndex("dbo.GameParticipantPlayers", new[] { "GameParticipantId" });
            AlterColumn("dbo.GameParticipantPlayers", "GameParticipantId", c => c.Int());
            RenameColumn(table: "dbo.GameParticipantPlayers", name: "GameParticipantId", newName: "GameParticipant_Id");
            CreateIndex("dbo.GameParticipantPlayers", "GameParticipant_Id");
            AddForeignKey("dbo.GameParticipantPlayers", "GameParticipant_Id", "dbo.GameParticipants", "Id");
        }
    }
}
