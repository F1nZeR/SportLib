namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedParentEventTypeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventTypes", "ParentEventTypeId", c => c.Int());
            CreateIndex("dbo.EventTypes", "ParentEventTypeId");
            AddForeignKey("dbo.EventTypes", "ParentEventTypeId", "dbo.EventTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventTypes", "ParentEventTypeId", "dbo.EventTypes");
            DropIndex("dbo.EventTypes", new[] { "ParentEventTypeId" });
            DropColumn("dbo.EventTypes", "ParentEventTypeId");
        }
    }
}
