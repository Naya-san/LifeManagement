using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using LifeManagement.ViewModels;

namespace LifeManagement.Logic
{
    public static class EfficiencyCalculator
    {
        private const double ImpotantWork = 1;
        private const double ImpotantCompleate = 1;
        private const double ImpotantWorkAndCompleate = 1;
        private const double OverdueWork = 1;
        private const double OverdueNoCompleate = 3;

        public static  double  CalculateCompleateLevel(DateTime now, ListForDay listForDay)
        {
            var db = new ApplicationDbContext();
            var settings = db.UserSettings.FirstOrDefault(x => x.UserId == listForDay.UserId);
            var nowS = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            double minutesForEvents = 0;
            foreach (var @event in listForDay.Events)
            {
                minutesForEvents += (((@event.EndDate.Value > now) ? now : @event.EndDate.Value) - (((@event.StartDate.Value < nowS) ? nowS : @event.StartDate.Value))).TotalMinutes;
                if (@event.OnBackground)
                {
                    minutesForEvents *= settings.ParallelismPercentage / 100.0;
                }
            }
            var freeTimeForTasks = settings.WorkingTime - TimeSpan.FromMinutes(minutesForEvents);
            double minutesForTasks = 0;
            double bonus = 0;
            double fines = 0;
            
            foreach (var archive in listForDay.Archive)
            {
                minutesForTasks += settings.GetAverageComplexityRange(archive.Task.Complexity).TotalMinutes*
                                   (archive.LevelOnEnd - archive.LevelOnStart)/100.0;
                if (archive.Task.EndDate.HasValue)
                {
                    if (archive.Task.EndDate.Value < now)
                    {
                        if (archive.LevelOnStart < archive.LevelOnEnd)
                        {
                            fines += OverdueWork;
                        }
                        else
                        {
                            fines += OverdueNoCompleate;
                        }
                    }
                    if (archive.Task.EndDate.Value > now)
                    {
                        bonus += (archive.LevelOnEnd - archive.LevelOnStart)/10.0;
                    }
                }
                if (archive.Task.IsImportant)
                {
                    if (archive.LevelOnStart < archive.LevelOnEnd)
                    {
                        bonus += ImpotantWork;
                        if (archive.LevelOnEnd == 100)
                        {
                            bonus += ImpotantCompleate;
                        }
                        if (archive.LevelOnStart < 25)
                        {
                            bonus += ImpotantWorkAndCompleate;
                        }
                    }
                }
            }
            return ((minutesForTasks * 100) / freeTimeForTasks.TotalMinutes)+bonus-fines;
        }
         public static  double  CalculateCompleateLevel(ToDoList taskList, TaskListSettingsViewModel listSettings, UserSetting setting)
        {
            double minutesForTasks = 0;
            double bonus = 0;
            var today = DateTime.UtcNow.Date ;
            foreach (var task in taskList.TasksTodo)
            {
                minutesForTasks += setting.GetAverageComplexityRange(task.Complexity).TotalMinutes;
                if (task.EndDate.HasValue && task.EndDate.Value > today)
                    {
                        bonus += 10;
                    }
                if (task.IsImportant)
                {
                    bonus += ImpotantWork;
                    bonus += ImpotantCompleate;
                    bonus += ImpotantWorkAndCompleate;
                }
            }
            return ((minutesForTasks * 100) / listSettings.TimeToFill.TotalMinutes)+bonus;
        }
    }
}