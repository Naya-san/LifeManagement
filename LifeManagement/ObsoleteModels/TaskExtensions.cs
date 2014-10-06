using System;
using System.Data.Entity;
using System.Linq;

namespace LifeManagement.ObsoleteModels
{
    public static class TaskExtensions
    {
        public static bool IsAddable(this Task task)
        {
            if ((task.Routines == null ||
                task.Routines.FirstOrDefault(
                    r =>
                        !r.IsDeleted &&
                        DateTime.Compare(r.StartDate,
                            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) >= 0) == null) &&
                            (task.ParentTask == null || task.ParentTask.IsAddable())
                            /*&& (task.ChildTasks == null || task.ChildTasks.Any(t => t.IsAddable()))*/)
            {
                return true;
            }
            return false;
        }

        public static void Delite(this Task task, LifeManagementContext db)
        {
            if (task.IsDeleted)
            {
                return;
            }
            task.IsDeleted = true;
            if (task.Routines != null)
            {
                foreach (var routine in task.Routines)
                {
                    if (!routine.IsDeleted)
                    {
                        routine.IsDeleted = true;
                        routine.UpdatedOn = DateTime.UtcNow;
                        db.Entry(routine).State = EntityState.Modified;
                    }
                }
            }
            if (task.ChildTasks != null)
            {
                foreach (var child in task.ChildTasks)
                {
                    child.Delite(db);
                }
            }

            task.UpdatedOn = DateTime.UtcNow;
            db.Entry(task).State = EntityState.Modified;
        }
    }
}