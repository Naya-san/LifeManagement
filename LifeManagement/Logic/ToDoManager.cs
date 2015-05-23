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
        private const double factor = 1.25;
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly TaskListSettingsViewModel listSettings;
        private readonly UserSetting userSetting;
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

        public async System.Threading.Tasks.Task<VersionsViewModel> Generate()
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

            return await GreedyAlgorithm();
        }

        private async System.Threading.Tasks.Task<VersionsViewModel> GreedyAlgorithm()
        {
            var today = DateTime.UtcNow;
            var applicantTasks = db.Records
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
                                (!x.StartDate.HasValue || (x.StartDate.HasValue && x.StartDate > listSettings.Date && x.IsImportant))
                            )
                            ||
                            (x.StartDate.HasValue && x.StartDate > listSettings.Date && x.IsImportant)
                            ||
                            (x.EndDate.HasValue && x.EndDate > listSettings.Date)
                        )
                 )
                 .ToList();
            var tasksTmp = applicantTasks.ToList();
            foreach (var task in tasksTmp)
            {
                if ((task.StartDate.HasValue && task.StartDate > listSettings.Date && task.IsImportant &&
                     task.StartDate.Value.Subtract(today).Days > 3)
                    ||
                    (task.EndDate.HasValue && task.EndDate > listSettings.Date  && task.EndDate.Value.Subtract(today).Days > 5)
                    )
                {
                    applicantTasks.Remove(task);
                }
            }
            var applicantTaskGroups = applicantTasks
                .OrderByDescending(x => x.CalculateTimeLeft(userSetting))
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
            var quontityOfComplexityGroupInUse = applicantTaskGroups.Count;
            foreach (var toDo in versions.ToDoLists)
            {
                int i = (int) toDo.TasksTodo.Last().Complexity;
                for (int j = 1; j <= quontityOfComplexityGroupInUse; j++)
                {
                    foreach (var task in applicantTaskGroups[(i + j) % quontityOfComplexityGroupInUse])
                    {
                        if ( listSettings.TimeToFill.Subtract(toDo.TimeEstimate).Ticks*factor >= task.CalculateTimeLeft(userSetting).Ticks && !toDo.ConteinsTask(task))
                        {
                            toDo.AddTask(task);
                        }
                    }
                }
            }

            Sort(versions);
            return RemoveTheSameVariants(versions);
        }
        public void Sort(VersionsViewModel versions)
        {
            foreach (var todo in versions.ToDoLists)
            {
                todo.Score = EfficiencyCalculator.CalculateCompleateLevel(todo, listSettings, userSetting);
                todo.SortTasks();
            }
            versions.ToDoLists = versions.ToDoLists.OrderByDescending(x => x.Score).ToList();
        }


        private VersionsViewModel GenerateFirstEtap(IGrouping<Complexity, Task> group)
        {
            var versions = new VersionsViewModel();
            var tasks = group.OrderByDescending(x => x.CalculateTimeLeft(userSetting)).ToList();
            foreach (var task in tasks)
            {
                if (task.CalculateTimeLeft(userSetting).Ticks < listSettings.TimeToFill.Ticks * factor || (task.IsImportant && task.IsUrgent(userSetting)))
                {
                    var list = new ToDoList(userSetting);
                    list.AddTask(task);
                    versions.ToDoLists.Add(list);
                }
            }
            return versions;
        }

        public VersionsViewModel RemoveTheSameVariants(VersionsViewModel versions)
        {
            if (versions== null || versions.IsEmpty())
            {
               return versions; 
            }
            var newVersions = new VersionsViewModel();
            foreach (var version in versions.ToDoLists)
            {
                if (!newVersions.ToDoLists.Contains(version))
                {
                    newVersions.ToDoLists.Add(version);
                }
            }
            return newVersions;
        }
    }
}