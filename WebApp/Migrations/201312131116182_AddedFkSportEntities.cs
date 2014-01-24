namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFkSportEntities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EventTypes", "Sport_Id", "dbo.Sports");
            DropForeignKey("dbo.PlayerPropertyTypes", "Sport_Id", "dbo.Sports");
            DropForeignKey("dbo.TeamPropertyTypes", "Sport_Id", "dbo.Sports");
            DropIndex("dbo.EventTypes", new[] { "Sport_Id" });
            DropIndex("dbo.PlayerPropertyTypes", new[] { "Sport_Id" });
            DropIndex("dbo.TeamPropertyTypes", new[] { "Sport_Id" });
            RenameColumn(table: "dbo.EventTypes", name: "Sport_Id", newName: "SportId");
            RenameColumn(table: "dbo.PlayerPropertyTypes", name: "Sport_Id", newName: "SportId");
            RenameColumn(table: "dbo.TeamPropertyTypes", name: "Sport_Id", newName: "SportId");
            AlterColumn("dbo.EventTypes", "SportId", c => c.Int(nullable: false));
            AlterColumn("dbo.PlayerPropertyTypes", "SportId", c => c.Int(nullable: false));
            AlterColumn("dbo.TeamPropertyTypes", "SportId", c => c.Int(nullable: false));
            CreateIndex("dbo.EventTypes", "SportId");
            CreateIndex("dbo.PlayerPropertyTypes", "SportId");
            CreateIndex("dbo.TeamPropertyTypes", "SportId");
            AddForeignKey("dbo.EventTypes", "SportId", "dbo.Sports", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PlayerPropertyTypes", "SportId", "dbo.Sports", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TeamPropertyTypes", "SportId", "dbo.Sports", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamPropertyTypes", "SportId", "dbo.Sports");
            DropForeignKey("dbo.PlayerPropertyTypes", "SportId", "dbo.Sports");
            DropForeignKey("dbo.EventTypes", "SportId", "dbo.Sports");
            DropIndex("dbo.TeamPropertyTypes", new[] { "SportId" });
            DropIndex("dbo.PlayerPropertyTypes", new[] { "SportId" });
            DropIndex("dbo.EventTypes", new[] { "SportId" });
            AlterColumn("dbo.TeamPropertyTypes", "SportId", c => c.Int());
            AlterColumn("dbo.PlayerPropertyTypes", "SportId", c => c.Int());
            AlterColumn("dbo.EventTypes", "SportId", c => c.Int());
            RenameColumn(table: "dbo.TeamPropertyTypes", name: "SportId", newName: "Sport_Id");
            RenameColumn(table: "dbo.PlayerPropertyTypes", name: "SportId", newName: "Sport_Id");
            RenameColumn(table: "dbo.EventTypes", name: "SportId", newName: "Sport_Id");
            CreateIndex("dbo.TeamPropertyTypes", "Sport_Id");
            CreateIndex("dbo.PlayerPropertyTypes", "Sport_Id");
            CreateIndex("dbo.EventTypes", "Sport_Id");
            AddForeignKey("dbo.TeamPropertyTypes", "Sport_Id", "dbo.Sports", "Id");
            AddForeignKey("dbo.PlayerPropertyTypes", "Sport_Id", "dbo.Sports", "Id");
            AddForeignKey("dbo.EventTypes", "Sport_Id", "dbo.Sports", "Id");
        }
    }
}
