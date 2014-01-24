namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedPlayerProperty : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PlayerProperties", "Player_Id", "dbo.Participants");
            DropIndex("dbo.PlayerProperties", new[] { "Player_Id" });
            AddColumn("dbo.PlayerProperties", "PlayerId", c => c.Int(nullable: false));
            CreateIndex("dbo.PlayerProperties", "PlayerId");
            AddForeignKey("dbo.PlayerProperties", "PlayerId", "dbo.Participants", "Id", cascadeDelete: false);
            DropColumn("dbo.PlayerProperties", "Player_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerProperties", "Player_Id", c => c.Int());
            DropForeignKey("dbo.PlayerProperties", "PlayerId", "dbo.Participants");
            DropIndex("dbo.PlayerProperties", new[] { "PlayerId" });
            DropColumn("dbo.PlayerProperties", "PlayerId");
            CreateIndex("dbo.PlayerProperties", "Player_Id");
            AddForeignKey("dbo.PlayerProperties", "Player_Id", "dbo.Participants", "Id");
        }
    }
}
