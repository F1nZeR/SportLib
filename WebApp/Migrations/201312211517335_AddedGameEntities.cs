namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGameEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameParticipants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParticipantId = c.Int(nullable: false),
                        GameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Participants", t => t.ParticipantId, cascadeDelete: false)
                .Index(t => t.GameId)
                .Index(t => t.ParticipantId);
            
            CreateTable(
                "dbo.GameParticipantPlayerProps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GameParticipantPlayerId = c.Int(nullable: false),
                        PlayerPropertyTypeId = c.Int(nullable: false),
                        PropValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameParticipantPlayers", t => t.GameParticipantPlayerId, cascadeDelete: false)
                .ForeignKey("dbo.PlayerPropertyTypes", t => t.PlayerPropertyTypeId, cascadeDelete: true)
                .Index(t => t.GameParticipantPlayerId)
                .Index(t => t.PlayerPropertyTypeId);
            
            CreateTable(
                "dbo.GameParticipantPlayers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GameParticipId = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                        GameParticipant_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameParticipants", t => t.GameParticipant_Id)
                .ForeignKey("dbo.Participants", t => t.PlayerId, cascadeDelete: false)
                .Index(t => t.GameParticipant_Id)
                .Index(t => t.PlayerId);
            
            AddColumn("dbo.Games", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Games", "Place", c => c.String(nullable: false));
            AddColumn("dbo.Games", "Description", c => c.String());
            AddColumn("dbo.Games", "WinnerId", c => c.Int());
            AddColumn("dbo.Games", "SportId", c => c.Int(nullable: false));
            CreateIndex("dbo.Games", "SportId");
            CreateIndex("dbo.Games", "WinnerId");
            AddForeignKey("dbo.Games", "SportId", "dbo.Sports", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Games", "WinnerId", "dbo.Participants", "Id");
            DropColumn("dbo.Games", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "Name", c => c.String());
            DropForeignKey("dbo.GameParticipantPlayerProps", "PlayerPropertyTypeId", "dbo.PlayerPropertyTypes");
            DropForeignKey("dbo.GameParticipantPlayerProps", "GameParticipantPlayerId", "dbo.GameParticipantPlayers");
            DropForeignKey("dbo.GameParticipantPlayers", "PlayerId", "dbo.Participants");
            DropForeignKey("dbo.GameParticipantPlayers", "GameParticipant_Id", "dbo.GameParticipants");
            DropForeignKey("dbo.Games", "WinnerId", "dbo.Participants");
            DropForeignKey("dbo.Games", "SportId", "dbo.Sports");
            DropForeignKey("dbo.GameParticipants", "ParticipantId", "dbo.Participants");
            DropForeignKey("dbo.GameParticipants", "GameId", "dbo.Games");
            DropIndex("dbo.GameParticipantPlayerProps", new[] { "PlayerPropertyTypeId" });
            DropIndex("dbo.GameParticipantPlayerProps", new[] { "GameParticipantPlayerId" });
            DropIndex("dbo.GameParticipantPlayers", new[] { "PlayerId" });
            DropIndex("dbo.GameParticipantPlayers", new[] { "GameParticipant_Id" });
            DropIndex("dbo.Games", new[] { "WinnerId" });
            DropIndex("dbo.Games", new[] { "SportId" });
            DropIndex("dbo.GameParticipants", new[] { "ParticipantId" });
            DropIndex("dbo.GameParticipants", new[] { "GameId" });
            DropColumn("dbo.Games", "SportId");
            DropColumn("dbo.Games", "WinnerId");
            DropColumn("dbo.Games", "Description");
            DropColumn("dbo.Games", "Place");
            DropColumn("dbo.Games", "Date");
            DropTable("dbo.GameParticipantPlayers");
            DropTable("dbo.GameParticipantPlayerProps");
            DropTable("dbo.GameParticipants");
        }
    }
}
