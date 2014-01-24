namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class extendsportaddplayerPropTypeteamPropType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayerPropertyTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        IsDependsOnGame = c.Boolean(nullable: false),
                        Sport_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sports", t => t.Sport_Id)
                .Index(t => t.Sport_Id);
            
            CreateTable(
                "dbo.TeamPropertyTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        Sport_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sports", t => t.Sport_Id)
                .Index(t => t.Sport_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamPropertyTypes", "Sport_Id", "dbo.Sports");
            DropForeignKey("dbo.PlayerPropertyTypes", "Sport_Id", "dbo.Sports");
            DropIndex("dbo.TeamPropertyTypes", new[] { "Sport_Id" });
            DropIndex("dbo.PlayerPropertyTypes", new[] { "Sport_Id" });
            DropTable("dbo.TeamPropertyTypes");
            DropTable("dbo.PlayerPropertyTypes");
        }
    }
}
