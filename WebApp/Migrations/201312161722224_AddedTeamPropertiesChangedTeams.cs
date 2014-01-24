namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTeamPropertiesChangedTeams : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false),
                        TeamPropertyTypeId = c.Int(nullable: false),
                        TeamId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Participants", t => t.TeamId, cascadeDelete: false)
                .ForeignKey("dbo.TeamPropertyTypes", t => t.TeamPropertyTypeId, cascadeDelete: true)
                .Index(t => t.TeamId)
                .Index(t => t.TeamPropertyTypeId);
            
            AddColumn("dbo.Participants", "Name1", c => c.String());
            AddColumn("dbo.Participants", "City", c => c.String());
            AddColumn("dbo.Participants", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamProperties", "TeamPropertyTypeId", "dbo.TeamPropertyTypes");
            DropForeignKey("dbo.TeamProperties", "TeamId", "dbo.Participants");
            DropIndex("dbo.TeamProperties", new[] { "TeamPropertyTypeId" });
            DropIndex("dbo.TeamProperties", new[] { "TeamId" });
            DropColumn("dbo.Participants", "Description");
            DropColumn("dbo.Participants", "City");
            DropColumn("dbo.Participants", "Name1");
            DropTable("dbo.TeamProperties");
        }
    }
}
