namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNameFieldToParticipant : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Participants", "Name", c => c.String(nullable: false));
            DropColumn("dbo.Participants", "Name1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Participants", "Name1", c => c.String());
            AlterColumn("dbo.Participants", "Name", c => c.String());
        }
    }
}
