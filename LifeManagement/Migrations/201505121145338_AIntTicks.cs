namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AIntTicks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserSettings", "TimeZoneShiftTicks", c => c.Long(nullable: false));
            DropColumn("dbo.UserSettings", "TimeZoneShift");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserSettings", "TimeZoneShift", c => c.Time(nullable: false, precision: 7));
            DropColumn("dbo.UserSettings", "TimeZoneShiftTicks");
        }
    }
}
