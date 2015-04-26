namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeToEvent : DbMigration
    {
        public override void Up()
        {
        //    RenameTable(name: "dbo.ListForDayRecords", newName: "ListForDays");
            DropForeignKey("dbo.ListForDayRecords", "ListForDay_Id", "dbo.ListForDays");
            DropForeignKey("dbo.ListForDayRecords", "Record_Id", "dbo.Records");
            DropIndex("dbo.ListForDayRecords", new[] { "ListForDay_Id" });
            DropIndex("dbo.ListForDayRecords", new[] { "Record_Id" });
            AddColumn("dbo.ListForDays", "Record_Id", c => c.Guid());
            AddColumn("dbo.Records", "ListForDay_Id", c => c.Guid());
            CreateIndex("dbo.Records", "ListForDay_Id");
            CreateIndex("dbo.ListForDays", "Record_Id");
            AddForeignKey("dbo.Records", "ListForDay_Id", "dbo.ListForDays", "Id");
            AddForeignKey("dbo.ListForDays", "Record_Id", "dbo.Records", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ListForDays", "Record_Id", "dbo.Records");
            DropForeignKey("dbo.Records", "ListForDay_Id", "dbo.ListForDays");
            DropIndex("dbo.ListForDays", new[] { "Record_Id" });
            DropIndex("dbo.Records", new[] { "ListForDay_Id" });
            DropColumn("dbo.Records", "ListForDay_Id");
            DropColumn("dbo.ListForDays", "Record_Id");
            CreateIndex("dbo.ListForDayRecords", "Record_Id");
            CreateIndex("dbo.ListForDayRecords", "ListForDay_Id");
            AddForeignKey("dbo.ListForDayRecords", "Record_Id", "dbo.Records", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ListForDayRecords", "ListForDay_Id", "dbo.ListForDays", "Id", cascadeDelete: true);
       //     RenameTable(name: "dbo.ListForDays", newName: "ListForDayRecords");
        }
    }
}
