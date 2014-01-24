namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTournaments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Place = c.String(),
                        Description = c.String(),
                        SportId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sports", t => t.SportId, cascadeDelete: true)
                .Index(t => t.SportId);
            
            CreateTable(
                "dbo.TournamentParticipants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TournamentId = c.Int(nullable: false),
                        ParticipantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Participants", t => t.ParticipantId, cascadeDelete: false)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.ParticipantId)
                .Index(t => t.TournamentId);
            
            AddColumn("dbo.Games", "TournamentId", c => c.Int());
            CreateIndex("dbo.Games", "TournamentId");
            AddForeignKey("dbo.Games", "TournamentId", "dbo.Tournaments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TournamentParticipants", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentParticipants", "ParticipantId", "dbo.Participants");
            DropForeignKey("dbo.Tournaments", "SportId", "dbo.Sports");
            DropForeignKey("dbo.Games", "TournamentId", "dbo.Tournaments");
            DropIndex("dbo.TournamentParticipants", new[] { "TournamentId" });
            DropIndex("dbo.TournamentParticipants", new[] { "ParticipantId" });
            DropIndex("dbo.Tournaments", new[] { "SportId" });
            DropIndex("dbo.Games", new[] { "TournamentId" });
            DropColumn("dbo.Games", "TournamentId");
            DropTable("dbo.TournamentParticipants");
            DropTable("dbo.Tournaments");
        }
    }
}
