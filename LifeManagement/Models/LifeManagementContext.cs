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

using System.Data.Entity;

namespace LifeManagement.Models
{
    public class LifeManagementContext : DbContext
    {
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<DayLimit> DayLimits { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; }
        public virtual DbSet<Routine> Routines { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TaskCategory> TaskCategories { get; set; }
        public virtual DbSet<Transfer> Transfers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<LifeManagementContext>());

            modelBuilder.Entity<TaskCategory>().HasKey(c => new { c.CategoryId, c.TaskId });
            modelBuilder.Entity<Task>().HasMany(c => c.TaskCategories).WithRequired().HasForeignKey(c => c.TaskId);
            modelBuilder.Entity<Category>().HasMany(c => c.TaskCategories).WithRequired().HasForeignKey(c => c.CategoryId);
            modelBuilder.Entity<Project>().HasMany(c => c.ChildProjects).WithOptional().HasForeignKey(c => c.ParentProjectId);
            modelBuilder.Entity<Project>().HasMany(c => c.Tasks).WithRequired().HasForeignKey(c => c.ProjectId);
            modelBuilder.Entity<Task>().HasMany(c => c.Attachments).WithRequired().HasForeignKey(c => c.TaskId);
            modelBuilder.Entity<Task>().HasMany(c => c.Comments).WithRequired().HasForeignKey(c => c.TaskId);
            modelBuilder.Entity<Task>().HasMany(c => c.Routines).WithRequired().HasForeignKey(c => c.TaskId);
            modelBuilder.Entity<Task>().HasMany(c => c.ChildTasks).WithOptional().HasForeignKey(c => c.ParentTaskId);
            modelBuilder.Entity<Task>().HasMany(c => c.Transfers).WithRequired().HasForeignKey(c => c.TaskId);
            modelBuilder.Entity<DayLimit>().HasMany(c => c.Routines).WithRequired().HasForeignKey(c => c.DayLimitId);
        }
    }
}
