namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TableSettingsUpdate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserSettings", "ComplexityMediumFrom");
            DropColumn("dbo.UserSettings", "ComplexityHightFrom");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserSettings", "ComplexityHightFrom", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.UserSettings", "ComplexityMediumFrom", c => c.Time(nullable: false, precision: 7));
        }
    }
}
