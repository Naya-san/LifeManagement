namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewEntetis : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ListForDays",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        CompleteLevel = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ComplexityLowFrom = c.Double(nullable: false),
                        ComplexityLowTo = c.Double(nullable: false),
                        ComplexityMediumFrom = c.Double(nullable: false),
                        ComplexityMediumTo = c.Double(nullable: false),
                        ComplexityHightFrom = c.Double(nullable: false),
                        ComplexityHightTo = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ListForDayRecords",
                c => new
                    {
                        ListForDay_Id = c.Guid(nullable: false),
                        Record_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ListForDay_Id, t.Record_Id })
                .ForeignKey("dbo.ListForDays", t => t.ListForDay_Id, cascadeDelete: true)
                .ForeignKey("dbo.Records", t => t.Record_Id, cascadeDelete: true)
                .Index(t => t.ListForDay_Id)
                .Index(t => t.Record_Id);
            
            AddColumn("dbo.Records", "OnBackground", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Settings", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ListForDays", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ListForDayRecords", "Record_Id", "dbo.Records");
            DropForeignKey("dbo.ListForDayRecords", "ListForDay_Id", "dbo.ListForDays");
            DropIndex("dbo.Settings", new[] { "UserId" });
            DropIndex("dbo.ListForDays", new[] { "UserId" });
            DropIndex("dbo.ListForDayRecords", new[] { "Record_Id" });
            DropIndex("dbo.ListForDayRecords", new[] { "ListForDay_Id" });
            DropColumn("dbo.Records", "OnBackground");
            DropTable("dbo.ListForDayRecords");
            DropTable("dbo.Settings");
            DropTable("dbo.ListForDays");
        }
    }
}
