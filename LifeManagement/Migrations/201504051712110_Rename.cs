namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Records", "IsImportant", c => c.Boolean(nullable: false));
            DropColumn("dbo.Records", "IsUrgent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Records", "IsUrgent", c => c.Boolean(nullable: false));
            DropColumn("dbo.Records", "IsImportant");
        }
    }
}
