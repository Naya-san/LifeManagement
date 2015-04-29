using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using Task = LifeManagement.Models.DB.Task;

namespace LifeManagement.Logic
{
    public class TodoListTimeTicker
    {
        private static int frequency = 30;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(frequency);
        private Timer _timer;
        private volatile ApplicationDbContext db = new ApplicationDbContext();

        private volatile bool _updating;
        private readonly object _updateLock = new object();
        public TodoListTimeTicker()
        {
            _timer = new Timer(UpdateTodoList, null, _updateInterval, _updateInterval);
        }

        private void UpdateTodoList(object state)
        {
            lock (_updateLock)
            {
                if (!_updating)
                {
                    _updating = true;
                    DateTime now = DateTime.UtcNow;
                    CreateNewListForDay(now);
                    CloseListForDay(now);
                    db.SaveChanges();
                    _updating = false;
                }
            }
        }

        private void CreateNewListForDay(DateTime now)
        {
            var startEtalon = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var userSettings = db.UserSettings.Where(x => (now.Add(x.TimeZoneShift) - startEtalon).Ticks > 0 || (now.Add(x.TimeZoneShift) - startEtalon).Ticks < _updateInterval.Ticks).ToList();
            foreach (var settings in userSettings)
            {
                var listForDay = new ListForDay(now.Date) {UserId = settings.UserId};
                var tasks =
                    db.Records.OfType<Task>().Where(x => x.UserId == settings.UserId && x.CompleteLevel < 100 &&
                        (((x.StartDate.HasValue && x.StartDate.Value.Date <= now) && ((x.EndDate.HasValue && x.EndDate.Value.Date >= now.Date) || !x.EndDate.HasValue))
                        ||
                        (!x.StartDate.HasValue && x.EndDate.HasValue && x.EndDate.Value.Date >= now.Date))).ToList();
                foreach (var task in tasks)
                {
                    listForDay.Archive.Add(new Archive(task));
                }
                db.ListsForDays.Add(listForDay);
            }
        }

        private async void CloseListForDay(DateTime now)
        {
            var closeEtalon = new DateTime(now.Year, now.Month, now.Day, 23, 59, 0);
            var userSettings = db.UserSettings.Where(x => closeEtalon - now.Add(x.TimeZoneShift) < _updateInterval || (closeEtalon - now.Add(x.TimeZoneShift)).Ticks > 0).ToList();
            foreach (var settings in userSettings)
            {
                var listForDay = await db.ListsForDays.Include(x => x.Archive).Include(x => x.Events).FirstOrDefaultAsync(x => x.UserId == settings.UserId && x.Date == now.Date);
                if (listForDay == null)
                {
                    continue;
                }
                var tasks =
                    db.Records.OfType<Task>().Where(x => x.UserId == settings.UserId && x.CompleteLevel < 100 &&
                        (((x.StartDate.HasValue && x.StartDate.Value.Date <= now) && ((x.EndDate.HasValue && x.EndDate.Value.Date >= now.Date) || !x.EndDate.HasValue))
                        ||
                        (!x.StartDate.HasValue && x.EndDate.HasValue && x.EndDate.Value.Date >= now.Date))).ToList();
                foreach (var task in tasks)
                {
                    var archive = listForDay.Archive.FirstOrDefault(x => x.TaskId == task.Id);
                    if (archive == null)
                    {
                        archive = new Archive(task);
                        archive.LevelOnStart = 0;
                        listForDay.Archive.Add(archive);
                    }
                    archive.LevelOnEnd = task.CompleteLevel;
                }
                var archiveListTmp = db.Archives.Include(x => x.Task).Where(x => listForDay.Archive.Contains(x)).ToList();
                foreach (var archive in archiveListTmp)
                {
                    if (!tasks.Contains(archive.Task))
                    {
                        listForDay.Archive.Remove(archive);
                    }
                }
                listForDay.Events = db.Records.OfType<Event>().Where(x => x.StartDate.Value.Date == now.Date || x.EndDate.Value.Date == now.Date || (x.StartDate.Value.Date <= now.Date && x.EndDate.Value.Date >= now.Date)).ToList();
                listForDay.CompleteLevel = await CalculateCompleateLevel(now, listForDay);

            }
        }

        private async Task<double> CalculateCompleateLevel(DateTime now, ListForDay listForDay)
        {
            var settings = await db.UserSettings.FirstOrDefaultAsync(x => x.UserId == listForDay.UserId);
            var nowS = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            double minutesForEvents  = 0;
            foreach (var @event in listForDay.Events)
            {
                minutesForEvents +=(((@event.EndDate.Value > now) ? now : @event.EndDate.Value) - (((@event.StartDate.Value < nowS) ? nowS : @event.StartDate.Value))).TotalMinutes;
                if (@event.OnBackground)
                {
                    minutesForEvents *= settings.ParallelismPercentage / 100.0;
                }
            }
            var freeTimeForTasks = settings.WorkingTime - TimeSpan.FromMinutes(minutesForEvents);
            double minutesForTasks  = listForDay.Archive.Sum(archive => settings.GetMinComplexityRange(archive.Task.Complexity).TotalMinutes*(archive.LevelOnEnd - archive.LevelOnStart)/100.0);
            return  (minutesForTasks*100)/ freeTimeForTasks.TotalMinutes;

        }
    }
}