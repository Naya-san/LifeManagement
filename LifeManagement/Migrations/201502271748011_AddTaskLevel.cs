namespace LifeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaskLevel : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Records", "Complexity", c => c.Int());
            //AddColumn("dbo.Records", "CompleteLevel", c => c.Byte());
            //CreateTable(
            //    "dbo.Alerts",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false),
            //            RecordId = c.Guid(nullable: false),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(),
            //            Date = c.DateTime(nullable: false),
            //            Position = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Records", t => t.RecordId, cascadeDelete: true)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.RecordId)
            //    .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.Tags",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(nullable: false, maxLength: 25),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.AspNetUsers",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            UserName = c.String(),
            //            PasswordHash = c.String(),
            //            SecurityStamp = c.String(),
            //            Discriminator = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.Id);

            //CreateTable(
            //    "dbo.AspNetUserClaims",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            ClaimType = c.String(),
            //            ClaimValue = c.String(),
            //            User_Id = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
            //    .Index(t => t.User_Id);

            //CreateTable(
            //    "dbo.AspNetUserLogins",
            //    c => new
            //        {
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            LoginProvider = c.String(nullable: false, maxLength: 128),
            //            ProviderKey = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.AspNetUserRoles",
            //    c => new
            //        {
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            RoleId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.UserId, t.RoleId })
            //    .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.RoleId)
            //    .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.AspNetRoles",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);

            //CreateTable(
            //    "dbo.Projects",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false),
            //            ParentProjectId = c.Guid(),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(nullable: false, maxLength: 25),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Projects", t => t.ParentProjectId)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.ParentProjectId)
            //    .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.Records",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(nullable: false, maxLength: 25),
            //            Note = c.String(maxLength: 700),
            //            StartDate = c.DateTime(),
            //            EndDate = c.DateTime(),
            //            IsUrgent = c.Boolean(nullable: false),
            //            ProjectId = c.Guid(),
            //            CompletedOn = c.DateTime(),
            //            GroupId = c.Guid(),
            //            RepeatPosition = c.Int(),
            //            Complexity = c.Int(),
            //            CompleteLevel = c.Byte(),
            //            StopRepeatDate = c.DateTime(),
            //            Discriminator = c.String(nullable: false, maxLength: 128),

            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: false)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
            //    .Index(t => t.ProjectId)
            //    .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.Feedbacks",
            //    c => new
            //        {
            //            Id = c.Guid(nullable: false),
            //            UserId = c.String(maxLength: 128),
            //            Subject = c.String(),
            //            Message = c.String(),
            //            Date = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId)
            //    .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.RecordTags",
            //    c => new
            //        {
            //            Record_Id = c.Guid(nullable: false),
            //            Tag_Id = c.Guid(nullable: false),
            //        })
            //    .PrimaryKey(t => new { t.Record_Id, t.Tag_Id })
            //    .ForeignKey("dbo.Records", t => t.Record_Id, cascadeDelete: true)
            //    .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
            //    .Index(t => t.Record_Id)
            //    .Index(t => t.Tag_Id);

        }

        public override void Down()
        {
            //DropColumn("dbo.Records", "CompleteLevel");
            //DropColumn("dbo.Records", "Complexity");
            //DropForeignKey("dbo.Feedbacks", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Alerts", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Alerts", "RecordId", "dbo.Records");
            //DropForeignKey("dbo.Records", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.RecordTags", "Tag_Id", "dbo.Tags");
            //DropForeignKey("dbo.RecordTags", "Record_Id", "dbo.Records");
            //DropForeignKey("dbo.Tags", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Tags", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Records", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Projects", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Projects", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Records", "ProjectId", "dbo.Projects");
            //DropForeignKey("dbo.Records", "ProjectId", "dbo.Projects");
            //DropForeignKey("dbo.Projects", "ParentProjectId", "dbo.Projects");
            //DropForeignKey("dbo.Projects", "ParentProjectId", "dbo.Projects");
            //DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            //DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Alerts", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Alerts", "RecordId", "dbo.Records");
            //DropIndex("dbo.Feedbacks", new[] { "UserId" });
            //DropIndex("dbo.Alerts", new[] { "UserId" });
            //DropIndex("dbo.Alerts", new[] { "RecordId" });
            //DropIndex("dbo.Records", new[] { "UserId" });
            //DropIndex("dbo.RecordTags", new[] { "Tag_Id" });
            //DropIndex("dbo.RecordTags", new[] { "Record_Id" });
            //DropIndex("dbo.Tags", new[] { "UserId" });
            //DropIndex("dbo.Tags", new[] { "UserId" });
            //DropIndex("dbo.Records", new[] { "UserId" });
            //DropIndex("dbo.Projects", new[] { "UserId" });
            //DropIndex("dbo.Projects", new[] { "UserId" });
            //DropIndex("dbo.Records", new[] { "ProjectId" });
            //DropIndex("dbo.Records", new[] { "ProjectId" });
            //DropIndex("dbo.Projects", new[] { "ParentProjectId" });
            //DropIndex("dbo.Projects", new[] { "ParentProjectId" });
            //DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            //DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            //DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            //DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            //DropIndex("dbo.Alerts", new[] { "UserId" });
            //DropIndex("dbo.Alerts", new[] { "RecordId" });
            //DropTable("dbo.RecordTags");
            //DropTable("dbo.Feedbacks");
            //DropTable("dbo.Records");
            //DropTable("dbo.Projects");
            //DropTable("dbo.AspNetRoles");
            //DropTable("dbo.AspNetUserRoles");
            //DropTable("dbo.AspNetUserLogins");
            //DropTable("dbo.AspNetUserClaims");
            //DropTable("dbo.AspNetUsers");
            //DropTable("dbo.Tags");
            //DropTable("dbo.Alerts");
        }
    }
}
