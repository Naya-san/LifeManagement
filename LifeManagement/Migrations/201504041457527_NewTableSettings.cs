namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTableSettings : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Settings", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Settings", new[] { "UserId" });
            CreateTable(
                "dbo.UserSettings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ComplexityLowFrom = c.Time(nullable: false, precision: 7),
                        ComplexityLowTo = c.Time(nullable: false, precision: 7),
                        ComplexityMediumFrom = c.Time(nullable: false, precision: 7),
                        ComplexityMediumTo = c.Time(nullable: false, precision: 7),
                        ComplexityHightFrom = c.Time(nullable: false, precision: 7),
                        ComplexityHightTo = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            DropTable("dbo.Settings");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.UserSettings", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserSettings", new[] { "UserId" });
            DropTable("dbo.UserSettings");
            CreateIndex("dbo.Settings", "UserId");
            AddForeignKey("dbo.Settings", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
