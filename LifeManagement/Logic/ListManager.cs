using LifeManagement.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LifeManagement.Enums;
using LifeManagement.Models;
using LifeManagement.Resources;
using LifeManagement.ViewModels;

namespace LifeManagement.Logic
{
    public static class ListManager
    {
        public static readonly double SuccessLevel = 65.5;
        public static async System.Threading.Tasks.Task<List<Task>> Generate(ApplicationDbContext db, TaskListSettingsViewModel listSetting)
        {
            var recordsOnDate =
                db.Records.Where(x => x.UserId == listSetting.UserId &&
                    (((x.StartDate.HasValue && x.StartDate.Value.Date <= listSetting.Date) && ((x.EndDate.HasValue && x.EndDate.Value.Date >= listSetting.Date) || !x.EndDate.HasValue))
                    ||
                    (!x.StartDate.HasValue && x.EndDate.HasValue && x.EndDate.Value.Date >= listSetting.Date))).ToList();
            var settings = db.UserSettings.FirstOrDefault(x => x.UserId == listSetting.UserId) ?? new UserSetting(listSetting.UserId);
            var freeTime = settings.WorkingTime.Subtract(new TimeSpan(recordsOnDate.Sum(record => record.CalculateTimeLeft(settings).Ticks)));
            if (freeTime.Ticks <= 0)
            {
                throw new Exception(ResourceScr.ErrorNoTime);
            }
            listSetting.TimeToFill = (freeTime.Ticks < listSetting.TimeToFill.Ticks) ? freeTime : listSetting.TimeToFill;
            var showcase = db.ListsForDays.Where(x => x.UserId == listSetting.UserId && x.CompleteLevel >= SuccessLevel).OrderByDescending(x => x.CompleteLevel).ToList();
            if (showcase.Any())
            {
                 return await GenerateListWithShowcase(db, listSetting, showcase);
            }
            return await GenerateListWithIntuition(db, listSetting); 
        }

        private static async System.Threading.Tasks.Task<List<Task>> GenerateListWithShowcase(ApplicationDbContext db,
            TaskListSettingsViewModel listSetting, List<ListForDay> showcase)
        {
            var tasks= new List<Task>();
            var userSettings = await db.UserSettings.FirstOrDefaultAsync(x => x.UserId == listSetting.UserId) ?? new UserSetting();
            if (showcase.Count > 5)
            {
                var showcaseQualification = showcase.Where(x => x.Date.DayOfWeek == listSetting.Date.DayOfWeek || listSetting.TimeToFill <= x.TaskTime(userSettings)).ToList();
                if (showcaseQualification.Count > 2)
                {
                    showcase = showcaseQualification;
                }
                else
                {
                    showcase = showcase.Take(5).ToList();
                }
            }
            var minutesAverage = CalculateIdealRatio(showcase, userSettings);
            var applicantTaskGroups = db.Records
            .Where(x => x.UserId == listSetting.UserId)
            .OfType<Task>()
            .Where(
                    x => !x.CompletedOn.HasValue &&
                    (
                        (!x.StartDate.HasValue && !x.EndDate.HasValue) 
                        ||
                        (
                            x.EndDate.HasValue && x.EndDate >= listSetting.Date
                            &&
                            (!x.StartDate.HasValue || (x.StartDate.HasValue && x.StartDate > listSetting.Date && x.IsImportant))
                        )
                    )
                )
            .OrderByDescending(x => x.IsImportant)
            .ThenByDescending(x => x.CompleteLevel)
            .ThenBy(x => x.EndDate)
            .GroupBy(x => x.Complexity)
            .ToList();
            var tasksLow = new List<Task>();
            var tasksMedium = new List<Task>();
            var tasksHight = new List<Task>();
            var tasksNone = new List<Task>();
            foreach(var group in applicantTaskGroups){
                foreach(var task in group)
                {
                   
                }
            }
            return tasks;
        }

        private static  double[] CalculateIdealRatio(List<ListForDay> showcase, UserSetting userSettings)
        {
                    var minutesLow = new List<double>();
                    var minutesNone = new List<double>();
                    var minutesMidium = new List<double>();
                    var minutesHight = new List<double>();
                    for (int i = 0; i < showcase.Count; i++)
                    {
                        minutesLow.Add(0);
                        minutesMidium.Add(0);
                        minutesHight.Add(0);
                        minutesNone.Add(0);
                        foreach (var archive in showcase[i].Archive)
                        {
                            switch (archive.Task.Complexity)
                            {
                                case Complexity.None:
                                    minutesNone[i] += archive.GetDurationEstimation(userSettings);
                                    break;
                                case Complexity.Low:
                                    minutesLow[i] += archive.GetDurationEstimation(userSettings);
                                    break;
                                case Complexity.Medium:
                                    minutesMidium[i] += archive.GetDurationEstimation(userSettings);
                                    break;
                                case Complexity.Hight:
                                    minutesHight[i] += archive.GetDurationEstimation(userSettings);
                                    break;
                            }
                        }     
                    }
                    return new[]
                        {
                            minutesNone.Sum()/minutesNone.Count,
                            minutesLow.Sum()/minutesLow.Count,
                            minutesMidium.Sum()/minutesMidium.Count,
                            minutesHight.Sum()/minutesHight.Count,
                        }
        }
        private static async System.Threading.Tasks.Task<List<Task>> GenerateListWithIntuition(ApplicationDbContext db, TaskListSettingsViewModel listSetting)
        {
            var tasks = new List<Task>();
            var userSettings = await db.UserSettings.FirstOrDefaultAsync(x => x.UserId == listSetting.UserId) ?? new UserSetting();
            var minutesTakeTask = 0.0;
            var records = db.Records
                .Where(x => x.UserId == listSetting.UserId)
                .OfType<Task>()
                .Where(
                        x => !x.CompletedOn.HasValue &&
                        (
                            (!x.StartDate.HasValue && !x.EndDate.HasValue) 
                            ||
                            (
                                x.EndDate != null 
                                && 
                                x.EndDate <= listSetting.Date.Add(x.CalculateTimeLeft(userSettings)).Add(x.CalculateTimeLeft(userSettings))
                                && 
                                x.EndDate > listSetting.Date 
                                &&
                                (x.StartDate == null || (x.StartDate != null && x.StartDate > listSetting.Date && x.IsImportant))
                            )
                        )
                 )
                .OrderByDescending(x => x.IsImportant)
                .ThenByDescending(x => x.CompleteLevel)
                .ThenBy(x => x.EndDate)
                .ToList();
            foreach(var task in records)
            {
                tasks.Add(task);
                minutesTakeTask+=task.CalculateTimeLeft(userSettings).TotalMinutes;
                if(minutesTakeTask < listSetting.TimeToFill.TotalMinutes)
                {
                    break;
                }
            }
            return tasks;
        }
     
    }
}