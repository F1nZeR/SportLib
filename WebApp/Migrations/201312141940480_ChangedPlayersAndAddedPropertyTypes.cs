namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    // я ошибс€ в названии. ƒобавлены PlayerProperties конечно же.
    public partial class ChangedPlayersAndAddedPropertyTypes : DbMigration
    {
        public override void Up()
        {
            //Sql("DELETE FROM dbo.Participants WHERE 1=1");
            //Sql("DELETE FROM dbo.Sports WHERE 1=1");
            CreateTable(
                "dbo.PlayerProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false),
                        PlayerPropertyTypeId = c.Int(nullable: false),
                        Player_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlayerPropertyTypes", t => t.PlayerPropertyTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Participants", t => t.Player_Id)
                .Index(t => t.PlayerPropertyTypeId)
                .Index(t => t.Player_Id);
            
            AddColumn("dbo.Participants", "SportId", c => c.Int(nullable: false));
            AddColumn("dbo.Participants", "Name", c => c.String());
            AddColumn("dbo.Participants", "Sex", c => c.Int());
            AddColumn("dbo.Participants", "Nationality", c => c.String());
            AddColumn("dbo.Participants", "Biography", c => c.String());
            CreateIndex("dbo.Participants", "SportId");
            AddForeignKey("dbo.Participants", "SportId", "dbo.Sports", "Id", cascadeDelete: true);
            DropColumn("dbo.Participants", "FirstName");
            DropColumn("dbo.Participants", "MiddleName");
            DropColumn("dbo.Participants", "LastName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Participants", "LastName", c => c.String());
            AddColumn("dbo.Participants", "MiddleName", c => c.String());
            AddColumn("dbo.Participants", "FirstName", c => c.String());
            DropForeignKey("dbo.PlayerProperties", "Player_Id", "dbo.Participants");
            DropForeignKey("dbo.PlayerProperties", "PlayerPropertyTypeId", "dbo.PlayerPropertyTypes");
            DropForeignKey("dbo.Participants", "SportId", "dbo.Sports");
            DropIndex("dbo.PlayerProperties", new[] { "Player_Id" });
            DropIndex("dbo.PlayerProperties", new[] { "PlayerPropertyTypeId" });
            DropIndex("dbo.Participants", new[] { "SportId" });
            DropColumn("dbo.Participants", "Biography");
            DropColumn("dbo.Participants", "Nationality");
            DropColumn("dbo.Participants", "Sex");
            DropColumn("dbo.Participants", "Name");
            DropColumn("dbo.Participants", "SportId");
            DropTable("dbo.PlayerProperties");
        }
    }
}
