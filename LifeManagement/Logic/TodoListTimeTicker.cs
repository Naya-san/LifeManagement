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
        private static int frequency = 60;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(frequency);
        private volatile ApplicationDbContext db = new ApplicationDbContext();

        public TodoListTimeTicker()
        {
            var t = System.Threading.Tasks.Task.Factory.StartNew(() => UpdateTodoList());
        }

        private async System.Threading.Tasks.Task UpdateTodoList()
        {
            while (true)
            {
                DateTime now = DateTime.UtcNow;
                await CreateNewListForDay(now);
                await CloseListForDay(now);
               
                Thread.Sleep(_updateInterval);
            }
        }

        private bool IsStartEtalon(DateTime now, UserSetting userSetting)
        {
            var startEtalon = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var diff = now.Add(userSetting.TimeZoneShift) - startEtalon;
       //     return (diff.Ticks > 0);
            return (diff.Ticks > 0 && diff < _updateInterval);
        }

        private async System.Threading.Tasks.Task CreateNewListForDay(DateTime now)
        {
            var userSettingsAll = new List<UserSetting>();
            var userSettings = new List<UserSetting>();
            var users = db.Users.ToList();
            foreach (var user in users)
            {
                var user1 = user;
                var settings = await db.UserSettings.FirstOrDefaultAsync(x => x.UserId == user1.Id);
                if (settings == null)
                {
                    settings = new UserSetting(user.Id);
                    db.UserSettings.Add(settings);
                    userSettings.Add(settings);
                }
                else
                {
                    userSettingsAll.Add(settings);
                }
            }
            foreach (var settings in userSettingsAll)
            {
                if (IsStartEtalon(now, settings))
                {
                    userSettings.Add(settings);
                }
            }

            foreach (var settings in userSettings)
            {
                var listForDay = new ListForDay(now.Date) {UserId = settings.UserId};
                var tasks =
                    db.Records.OfType<Task>().Where(
                        x => x.UserId == settings.UserId && x.CompleteLevel < 100 &&
                        (
                            (
                                (x.StartDate.HasValue && x.StartDate.Value.Year <= now.Year && x.StartDate.Value.Month <= now.Month && x.StartDate.Value.Day <= now.Day) 
                                ||
                                (x.EndDate.HasValue && x.EndDate.Value.Year <= now.Year && x.EndDate.Value.Month <= now.Month && x.EndDate.Value.Day <= now.Day)
                            )
                        )
                        ).ToList();
                foreach (var task in tasks)
                {
                    listForDay.Archive.Add(new Archive(task));
                }
                db.ListsForDays.Add(listForDay);
            }
            await db.SaveChangesAsync();
        }

        private bool IsCloseEtalon(DateTime now, UserSetting userSetting)
        {
            var closeEtalon = new DateTime(now.Year, now.Month, now.Day, 23, 59, 0);
            var diff = closeEtalon - now.Add(userSetting.TimeZoneShift);
        //    return true;
            return (diff < _updateInterval && diff.Ticks > 0);
        }
        private async System.Threading.Tasks.Task CloseListForDay(DateTime now)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var userSettingsAll = db.UserSettings.ToList();
            var userSettings = new List<UserSetting>();
            foreach (var settings in userSettingsAll)
            {
                if (IsCloseEtalon(now, settings))
                {
                    userSettings.Add(settings);
                }
            }
            foreach (var settings in userSettings)
            {
                var listForDay = await db.ListsForDays.Include(x => x.Archive).Include(x => x.Events).FirstOrDefaultAsync(x => x.UserId == settings.UserId && x.Date == now.Date);
                if (listForDay == null)
                {
                    continue;
                }
                var tasks =
                     db.Records.OfType<Task>().Where(
                        x => x.UserId == settings.UserId && 
                        (
                            (x.CompletedOn.HasValue && x.CompletedOn.Value.Day == now.Day &&  x.CompletedOn.Value.Month == now.Month &&  x.CompletedOn.Value.Year == now.Year)
                            ||
                            (
                                x.CompleteLevel < 100
                                &&
                                (
                                    (x.StartDate.HasValue && x.StartDate.Value.Year <= now.Year && x.StartDate.Value.Month <= now.Month && x.StartDate.Value.Day <= now.Day)
                                    ||
                                    (x.EndDate.HasValue && x.EndDate.Value.Year <= now.Year && x.EndDate.Value.Month <= now.Month && x.EndDate.Value.Day <= now.Day)
                                )
                            )
                        )
                      ).ToList();
                foreach (var task in tasks)
                {
                    var archive = listForDay.Archive.FirstOrDefault(x => x.TaskId == task.Id);
                    if (archive == null)
                    {
                        archive = new Archive(task) {LevelOnStart = 0};
                        listForDay.Archive.Add(archive);
                        db.Archives.Add(archive);
                    }
                    archive.LevelOnEnd = task.CompleteLevel;
                }
                var archiveListTmp = listForDay.Archive.ToList();
                foreach (var archive in archiveListTmp)
                {
                    var archive1 = archive;
                    var task = await db.Records.OfType<Task>().FirstAsync(x => x.Id == archive1.TaskId);
                    if (!tasks.Contains(task))
                    {
                        if (task.CompleteLevel == archive.LevelOnStart)
                        {
                            listForDay.Archive.Remove(archive);
                            db.Archives.Remove(archive);
                        }
                        else
                        {
                            archive.LevelOnEnd = task.CompleteLevel;
                        }
                    }
                }
                listForDay.Events = db.Records.OfType<Event>().Where(
                    x =>
                        (x.StartDate.Value.Year <= now.Year && x.StartDate.Value.Month <= now.Month && x.StartDate.Value.Year <= now.Month)
                        &&
                        (x.EndDate.Value.Year >= now.Year && x.EndDate.Value.Month >= now.Month && x.EndDate.Value.Day >= now.Day)
                        )
                        .ToList();
                listForDay.CompleteLevel = await CalculateCompleateLevel(now, listForDay);
            }
            await db.SaveChangesAsync();
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