namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testdataAnnotationsforsport : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sports", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Sports", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Sports", "TimePeriodName", c => c.String(nullable: false));
            AlterColumn("dbo.Sports", "TimePeriodCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sports", "TimePeriodCount", c => c.String());
            AlterColumn("dbo.Sports", "TimePeriodName", c => c.String());
            AlterColumn("dbo.Sports", "Description", c => c.String());
            AlterColumn("dbo.Sports", "Name", c => c.String());
        }
    }
}
