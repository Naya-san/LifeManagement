namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Archives",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TaskId = c.Guid(nullable: false),
                        LevelOnStart = c.Int(nullable: false),
                        LevelOnEnd = c.Int(nullable: false),
                        ListForDay_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Records", t => t.TaskId, cascadeDelete: true)
                .ForeignKey("dbo.ListForDays", t => t.ListForDay_Id)
                .Index(t => t.TaskId)
                .Index(t => t.ListForDay_Id);
            
            AddColumn("dbo.UserSettings", "TimeZoneShift", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Archives", "ListForDay_Id", "dbo.ListForDays");
            DropForeignKey("dbo.Archives", "TaskId", "dbo.Records");
            DropIndex("dbo.Archives", new[] { "ListForDay_Id" });
            DropIndex("dbo.Archives", new[] { "TaskId" });
            DropColumn("dbo.UserSettings", "TimeZoneShift");
            DropTable("dbo.Archives");
        }
    }
}
