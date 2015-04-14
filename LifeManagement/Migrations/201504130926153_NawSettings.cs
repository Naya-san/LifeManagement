namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NawSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserSettings", "ParallelismPercentage", c => c.Int(nullable: false));
            AddColumn("dbo.UserSettings", "WorkingTime", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserSettings", "WorkingTime");
            DropColumn("dbo.UserSettings", "ParallelismPercentage");
        }
    }
}
