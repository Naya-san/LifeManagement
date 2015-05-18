using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LifeManagement.Enums;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using LifeManagement.Resources;
using LifeManagement.ViewModels;

namespace LifeManagement.Logic
{
    public static class ToDoManager
    {
        private static readonly double SuccessLevel = 65.5;
        private static readonly double factor = 1.25;
        private static ApplicationDbContext db = new ApplicationDbContext();

        private static TimeSpan TimeTillTomorrow(TaskListSettingsViewModel listSetting)
        {
            var todayCheck = DateTime.UtcNow;
            return listSetting.Date.AddDays(1).Subtract(todayCheck);
        }

        public static VersionsViewModel Generate(TaskListSettingsViewModel listSetting)
        {
            var settings = db.UserSettings.FirstOrDefault(x => x.UserId == listSetting.UserId) ?? new UserSetting(listSetting.UserId);
            var recordsOnDate =
                db.Records.Where(x => x.UserId == listSetting.UserId &&
                    (
                     (x.StartDate.HasValue && x.StartDate.Value.Day <= listSetting.Date.Day && x.StartDate.Value.Month <= listSetting.Date.Month && x.StartDate.Value.Year <= listSetting.Date.Year)
                        ||
                     (!x.StartDate.HasValue && x.EndDate.HasValue && x.EndDate.Value.Day == listSetting.Date.Day && x.EndDate.Value.Month == listSetting.Date.Month && x.EndDate.Value.Year == listSetting.Date.Year)
                    )
                ).ToList();

            var freeTime = settings.WorkingTime.Subtract(new TimeSpan(recordsOnDate.Sum(record => record.CalculateTimeLeft(settings).Ticks)));
            var todayCheck = DateTime.UtcNow;

            if (freeTime.Ticks <= 0 || (todayCheck.Date == listSetting.Date && TimeTillTomorrow(listSetting) < settings.GetMinComplexityRange(Complexity.Low)))
            {
                throw new Exception(ResourceScr.ErrorNoTime);
            }
            listSetting.TimeToFill = (freeTime.Ticks < listSetting.TimeToFill.Ticks) ? freeTime : listSetting.TimeToFill;
            listSetting.TimeToFill = (todayCheck.Date == listSetting.Date && TimeTillTomorrow(listSetting) < listSetting.TimeToFill) ? TimeTillTomorrow(listSetting) : listSetting.TimeToFill;
            
        }

        private static async System.Threading.Tasks.Task<VersionsViewModel> GreedyAlgorithm(TaskListSettingsViewModel listSetting)
        {
            var userSettings = await db.UserSettings.FirstOrDefaultAsync(x => x.UserId == listSetting.UserId) ?? new UserSetting();
            var applicantTaskGroups = db.Records
                .Where(x => x.UserId == listSetting.UserId)
                .OfType<Task>()
                .Where(
                        x => !x.CompletedOn.HasValue &&
                        (
                            (!x.StartDate.HasValue && !x.EndDate.HasValue)
                            ||
                            (
                                x.EndDate.HasValue
                                &&
                                x.EndDate > listSetting.Date
                                &&
                                (!x.StartDate.HasValue || (x.StartDate.HasValue && x.StartDate > listSetting.Date && x.IsImportant))
                            )
                            ||
                            (x.StartDate.HasValue && x.StartDate > listSetting.Date && x.IsImportant)
                        )
                 )
                .OrderByDescending(x => x.IsImportant)
                .ThenByDescending(x => x.CompleteLevel)
                .ThenBy(x => x.EndDate)
                .GroupBy(x => x.Complexity)
                .OrderByDescending(x => x.Key)
                .ToList();
            VersionsViewModel versions = null;
            foreach (var @group in applicantTaskGroups)
            {
                versions = GenerateFirstEtap(@group, listSetting.TimeToFill, userSettings);
                if (!versions.IsEmpty())
                {
                    break;
                }
            }

            return versions;
        }

        private static VersionsViewModel GenerateFirstEtap(IGrouping<Complexity, Task> group,
            TimeSpan timeLimit, UserSetting userSettings)
        {
            var versions = new VersionsViewModel();
            foreach (var task in group)
            {
                if (task.CalculateTimeLeft(userSettings).Ticks < timeLimit.Ticks*factor)
                {
                    var list = new ToDoList(userSettings);
                    list.AddTask(task);
                    versions.ToDoLists.Add(list);
                }
            }
            return versions;
        }

        //private static async System.Threading.Tasks.Task<VersionsViewModel> GreedyAlgorithm(
        //    TaskListSettingsViewModel listSetting)
        //{
            
        //}

    }
}