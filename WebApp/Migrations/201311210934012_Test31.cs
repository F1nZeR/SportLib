namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Participants", "OwningTeam_Id", c => c.Int());
            CreateIndex("dbo.Participants", "OwningTeam_Id");
            AddForeignKey("dbo.Participants", "OwningTeam_Id", "dbo.Participants", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Participants", "OwningTeam_Id", "dbo.Participants");
            DropIndex("dbo.Participants", new[] { "OwningTeam_Id" });
            DropColumn("dbo.Participants", "OwningTeam_Id");
        }
    }
}
