namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFriendlyTournaments : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM dbo.Events");
            Sql("DELETE FROM dbo.GameParticipantPlayers");
            Sql("DELETE FROM dbo.GameParticipants");
            Sql("DELETE FROM dbo.Games");
            Sql("DELETE FROM dbo.Tournaments");
            DropForeignKey("dbo.Games", "TournamentId", "dbo.Tournaments");
            DropIndex("dbo.Games", new[] { "TournamentId" });
            AddColumn("dbo.Tournaments", "IsFriendlyTournament", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Games", "TournamentId", c => c.Int(nullable: false));
            CreateIndex("dbo.Games", "TournamentId");
            AddForeignKey("dbo.Games", "TournamentId", "dbo.Tournaments", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "TournamentId", "dbo.Tournaments");
            DropIndex("dbo.Games", new[] { "TournamentId" });
            AlterColumn("dbo.Games", "TournamentId", c => c.Int());
            DropColumn("dbo.Tournaments", "IsFriendlyTournament");
            CreateIndex("dbo.Games", "TournamentId");
            AddForeignKey("dbo.Games", "TournamentId", "dbo.Tournaments", "Id");
        }
    }
}
