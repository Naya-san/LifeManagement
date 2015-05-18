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
    public class ToDoManager
    {
        private static readonly double SuccessLevel = 65.5;
        private static readonly double factor = 1.25;
        private  ApplicationDbContext db = new ApplicationDbContext();
        private TaskListSettingsViewModel listSettings;
        private UserSetting userSetting;
        public ToDoManager(TaskListSettingsViewModel listSetting)
        {
            listSettings = listSetting;
            userSetting = db.UserSettings.FirstOrDefault(x => x.UserId == listSettings.UserId) ?? new UserSetting(listSettings.UserId);
        }

        private TimeSpan TimeTillTomorrow()
        {
            var todayCheck = DateTime.UtcNow;
            return listSettings.Date.AddDays(1).Subtract(todayCheck);
        }

        public VersionsViewModel Generate()
        {
           
            var recordsOnDate =
                db.Records.Where(x => x.UserId == listSettings.UserId &&
                    (
                     (x.StartDate.HasValue && x.StartDate.Value.Day <= listSettings.Date.Day && x.StartDate.Value.Month <= listSettings.Date.Month && x.StartDate.Value.Year <= listSettings.Date.Year)
                        ||
                     (!x.StartDate.HasValue && x.EndDate.HasValue && x.EndDate.Value.Day == listSettings.Date.Day && x.EndDate.Value.Month == listSettings.Date.Month && x.EndDate.Value.Year == listSettings.Date.Year)
                    )
                ).ToList();

            var freeTime = userSetting.WorkingTime.Subtract(new TimeSpan(recordsOnDate.Sum(record => record.CalculateTimeLeft(userSetting).Ticks)));
            var todayCheck = DateTime.UtcNow;

            if (freeTime.Ticks <= 0 || (todayCheck.Date == listSettings.Date && TimeTillTomorrow() < userSetting.GetMinComplexityRange(Complexity.Low)))
            {
                throw new Exception(ResourceScr.ErrorNoTime);
            }
            listSettings.TimeToFill = (freeTime.Ticks < listSettings.TimeToFill.Ticks) ? freeTime : listSettings.TimeToFill;
            listSettings.TimeToFill = (todayCheck.Date == listSettings.Date && TimeTillTomorrow() < listSettings.TimeToFill) ? TimeTillTomorrow() : listSettings.TimeToFill;
            
        }

        private async System.Threading.Tasks.Task<VersionsViewModel> GreedyAlgorithm()
        {
            var applicantTaskGroups = db.Records
                .Where(x => x.UserId == listSettings.UserId)
                .OfType<Task>()
                .Where(
                        x => !x.CompletedOn.HasValue &&
                        (
                            (!x.StartDate.HasValue && !x.EndDate.HasValue)
                            ||
                            (
                                x.EndDate.HasValue
                                &&
                                x.EndDate > listSettings.Date
                                &&
                                (!x.StartDate.HasValue || (x.StartDate.HasValue && x.StartDate > listSettings.Date && (x.IsImportant || IsUrgent(x))))
                            )
                            ||
                            (x.StartDate.HasValue && x.StartDate > listSettings.Date && (x.IsImportant || IsUrgent(x)))
                        )
                 )
                .OrderByDescending(x => x.IsImportant)
                .ThenByDescending(x => x.CompleteLevel)
                .ThenBy(x => x.EndDate)
                .GroupBy(x => x.Complexity)
                .OrderByDescending(x => x.Key)
                .ToList();
            var versions = new VersionsViewModel();
            foreach (var @group in applicantTaskGroups)
            {
                versions.Add(GenerateFirstEtap(@group));
                if (versions.Count() > 4)
                {
                    break;
                }
            }


            return versions;
        }

        private VersionsViewModel GenerateFirstEtap(IGrouping<Complexity, Task> group)
        {
            var versions = new VersionsViewModel();
            var tasks = group.OrderByDescending(x => x.CalculateTimeLeft(userSetting)).ToList();
            foreach (var task in tasks)
            {
                if (task.CalculateTimeLeft(userSetting).Ticks < listSettings.TimeToFill.Ticks*factor || (task.IsImportant && IsUrgent(task)))
                {
                    var list = new ToDoList(userSetting);
                    list.AddTask(task);
                    versions.ToDoLists.Add(list);
                }
            }
            return versions;
        }

        private bool IsUrgent(Task task)
        {
            if (!task.EndDate.HasValue)
            {
                return false;
            }

            return (DateTime.UtcNow.Subtract(task.EndDate.Value).Ticks < task.CalculateTimeLeft(userSetting).Ticks*6);
        }

        //private static async System.Threading.Tasks.Task<VersionsViewModel> GreedyAlgorithm(
        //    TaskListSettingsViewModel listSetting)
        //{
            
        //}

    }
}