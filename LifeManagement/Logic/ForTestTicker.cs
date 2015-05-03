using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LifeManagement.Enums;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using Task = LifeManagement.Models.DB.Task;

namespace LifeManagement.Logic
{
    public class ForTestTicker
    {
        private static int frequency = 12;
        private readonly TimeSpan _updateInterval = TimeSpan.FromHours(frequency);
        private volatile ApplicationDbContext db = new ApplicationDbContext();

        public ForTestTicker()
        {
            var t = System.Threading.Tasks.Task.Factory.StartNew(() => UpdateTodoList());
        }

        private async System.Threading.Tasks.Task UpdateTodoList()
        {
            while (true)
            {
                await UpdateTasks();
                Thread.Sleep(_updateInterval);
            }
        }

        private async System.Threading.Tasks.Task UpdateTasks()
        {
            DateTime now = DateTime.UtcNow;
            var tasks =
                db.Records.OfType<Task>().Where(
                    x =>
                        x.CompleteLevel < 100 && (
                            (x.StartDate.HasValue && x.StartDate.Value.Year <= now.Year &&
                             x.StartDate.Value.Month <= now.Month && x.StartDate.Value.Day <= now.Day)
                            ||
                            (x.EndDate.HasValue && x.EndDate.Value.Year <= now.Year &&
                             x.EndDate.Value.Month <= now.Month && x.EndDate.Value.Day <= now.Day)
                            )
                    ).ToList();
            var rand = new Random();
            foreach (var task in tasks)
            {
                task.CompleteLevel += rand.Next(0, 80);
                if (task.CompleteLevel < 100) continue;
                task.CompleteLevel = 100;
                task.CompletedOn = now;
            }
            await db.SaveChangesAsync();
        }
    }
}