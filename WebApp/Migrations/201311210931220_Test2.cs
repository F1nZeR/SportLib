namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Participants", "Age", c => c.Int());
            AddColumn("dbo.Participants", "PlayerCount", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Participants", "PlayerCount");
            DropColumn("dbo.Participants", "Age");
        }
    }
}
