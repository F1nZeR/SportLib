namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedeventtypeandsport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsSignifForProtocol = c.Boolean(nullable: false),
                        TotalChange = c.Int(nullable: false),
                        Sport_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sports", t => t.Sport_Id)
                .Index(t => t.Sport_Id);
            
            CreateTable(
                "dbo.Sports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        TimePeriodName = c.String(),
                        TimePeriodCount = c.String(),
                        SidesCountMin = c.Int(nullable: false),
                        SidesCountMax = c.Int(nullable: false),
                        IsTeamSport = c.Boolean(nullable: false),
                        TeamSizeMin = c.Int(nullable: false),
                        TeamSizeMax = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeamPlayers",
                c => new
                    {
                        Team_Id = c.Int(nullable: false),
                        Player_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Team_Id, t.Player_Id })
                .ForeignKey("dbo.Participants", t => t.Team_Id)
                .ForeignKey("dbo.Participants", t => t.Player_Id)
                .Index(t => t.Team_Id)
                .Index(t => t.Player_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamPlayers", "Player_Id", "dbo.Participants");
            DropForeignKey("dbo.TeamPlayers", "Team_Id", "dbo.Participants");
            DropForeignKey("dbo.EventTypes", "Sport_Id", "dbo.Sports");
            DropIndex("dbo.TeamPlayers", new[] { "Player_Id" });
            DropIndex("dbo.TeamPlayers", new[] { "Team_Id" });
            DropIndex("dbo.EventTypes", new[] { "Sport_Id" });
            DropTable("dbo.TeamPlayers");
            DropTable("dbo.Sports");
            DropTable("dbo.EventTypes");
        }
    }
}
