using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using LifeManagement.Models.DB;
using LifeManagement.Migrations;

namespace LifeManagement.Models
{
    public class LMContext : DbContext 
    {
        public virtual DbSet<Alert> Alerts { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Record> Records { get; set; }

        public LMContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            //base.OnModelCreating(modelBuilder);

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            //base.OnModelCreating(modelBuilder);

            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<LMContext>());

            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<Record>().HasMany(t => t.Tags).WithMany(t => t.Records);
            //modelBuilder.Entity<Record>().HasMany(c => c.Alerts).WithRequired().HasForeignKey(c => c.RecordId);
            //modelBuilder.Entity<Project>().HasMany(c => c.ChildProjects).WithOptional().HasForeignKey(c => c.ParentProjectId);
            //modelBuilder.Entity<Project>().HasMany(c => c.Tasks).WithRequired().HasForeignKey(c => c.ProjectId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Alerts).WithRequired().HasForeignKey(c => c.UserId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Projects).WithRequired().HasForeignKey(c => c.UserId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Records).WithRequired().HasForeignKey(c => c.UserId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Tags).WithRequired().HasForeignKey(c => c.UserId);
        }
    }
}