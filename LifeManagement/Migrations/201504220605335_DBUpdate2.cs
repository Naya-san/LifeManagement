namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBUpdate2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Archives", "ListForDay_Id", "dbo.ListForDays");
            DropIndex("dbo.Archives", new[] { "ListForDay_Id" });
            AddColumn("dbo.Archives", "TodoList_Id", c => c.Guid());
            CreateIndex("dbo.Archives", "TodoList_Id");
            AddForeignKey("dbo.Archives", "TodoList_Id", "dbo.ListForDays", "Id");
            DropColumn("dbo.Archives", "ListForDay_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Archives", "ListForDay_Id", c => c.Guid());
            DropForeignKey("dbo.Archives", "TodoList_Id", "dbo.ListForDays");
            DropIndex("dbo.Archives", new[] { "TodoList_Id" });
            DropColumn("dbo.Archives", "TodoList_Id");
            CreateIndex("dbo.Archives", "ListForDay_Id");
            AddForeignKey("dbo.Archives", "ListForDay_Id", "dbo.ListForDays", "Id");
        }
    }
}
