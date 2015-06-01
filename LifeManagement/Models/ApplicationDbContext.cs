#region License
// Copyright (c) 2014 Life Management Team
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Data.Entity;
using LifeManagement.Models.DB;
using Microsoft.AspNet.Identity.EntityFramework;
using LifeManagement.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.EnterpriseServices;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LifeManagement.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Alert> Alerts { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<UserSetting> UserSettings { get; set; }
        public virtual DbSet<ListForDay> ListsForDays { get; set; }
        public virtual DbSet<Archive> Archives { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        //    Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            base.OnModelCreating(modelBuilder);

            //Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
            //modelBuilder.Entity<IdentityUserLogin>().HasKey(x => x.UserId);
            //modelBuilder.Entity<IdentityUserRole>().HasKey(x => x.UserId);
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<Record>().HasMany(t => t.Tags).WithMany(t => t.Events);
            //modelBuilder.Entity<Record>().HasMany(c => c.Alerts).WithRequired().HasForeignKey(c => c.RecordId);
            //modelBuilder.Entity<Project>().HasMany(c => c.ChildProjects).WithOptional().HasForeignKey(c => c.ParentProjectId);
            //modelBuilder.Entity<Project>().HasMany(c => c.Tasks).WithRequired().HasForeignKey(c => c.ProjectId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Alerts).WithRequired().HasForeignKey(c => c.UserId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Projects).WithRequired().HasForeignKey(c => c.UserId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Events).WithRequired().HasForeignKey(c => c.UserId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Tags).WithRequired().HasForeignKey(c => c.UserId);
        }
    }
}