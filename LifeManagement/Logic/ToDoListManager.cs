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
    public static class ToDoListManager
    {
        public static readonly double SuccessLevel = 65.5;
        public static ApplicationDbContext db = new ApplicationDbContext();
        private static TimeSpan TimeTillTomorrow(TaskListSettingsViewModel listSetting)
        {
            var todayCheck = DateTime.UtcNow;
            return listSetting.Date.AddDays(1).Subtract(todayCheck);
        }

        public static async System.Threading.Tasks.Task<List<ToDoList>> Generate(TaskListSettingsViewModel listSetting)
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
            var showcase = db.ListsForDays.Where(x => x.UserId == listSetting.UserId && x.CompleteLevel >= SuccessLevel).OrderByDescending(x => x.CompleteLevel).ToList();
            if (showcase.Any())
            {
                return await GenerateListWithShowcase(listSetting, showcase);
            }
            return await GenerateListWithIntuition(listSetting);
        }

        private static List<ToDoList> BuildToDoListsFromBlock(List<List<ToDoList>> blocks, UserSetting userSettings)
        {
            var varients = new List<ToDoList>();
            int bestRow = ChooseBestRow(blocks);
            for (int i = 0; i < blocks[bestRow].Count; i++)
            {
                varients.Add(new ToDoList(userSettings));
                for (int j = 0; j < blocks.Count; j++)
                {
                    varients[i].AddTasksRange(blocks[j][i % blocks[j].Count].TasksTodo);
                }
            }
            return varients;
        }


        private static List<ToDoList> BuildToDoListsFromBlock(List<ToDoList>[][] blocks, UserSetting userSettings)
        {
            var varients = new List<ToDoList>();
            for (int i = 0; i < blocks.Count(); i++)
            {
                varients.Add(new ToDoList(userSettings));
                var tmp = new List<List<ToDoList>>();


            }
            return varients;
        }

        private static int ChooseBestRow(List<List<ToDoList>> blocks)
        {
            var indexRow = 0;
            for(var i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].Count >= 2 && blocks[i].Count < 6)
                {
                    indexRow = i;
                }
            }
            return indexRow;
        }

        private static List<ToDoList> GenerateBlocksFromGroups(IGrouping<Complexity, Task> group, double minutesAverage, UserSetting userSettings)
        {
            var blocksList= new List<ToDoList>();
            int index = 0;
            blocksList.Add(new ToDoList(userSettings));
            foreach (var task in group)
            {
                if (task.CalculateTimeLeft(userSettings).TotalMinutes > minutesAverage*1.2)
                {
                    continue;
                }
                if (blocksList[index].TimeEstimate.TotalMinutes >= minutesAverage)
                {
                    blocksList.Add(new ToDoList(userSettings));
                    index++;
                }
                blocksList[index].AddTask(task);
            }

            return blocksList;
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
            double averageAllTime = minutesNone.Sum()/minutesNone.Count +
                                    minutesLow.Sum()/minutesLow.Count +
                                    minutesMidium.Sum()/minutesMidium.Count +
                                    minutesHight.Sum()/minutesHight.Count;
            return new[]
                {
                    (minutesNone.Sum()/minutesNone.Count)*100/averageAllTime,
                    (minutesLow.Sum()/minutesLow.Count)*100/averageAllTime,
                    (minutesMidium.Sum()/minutesMidium.Count)*100/averageAllTime,
                    (minutesHight.Sum()/minutesHight.Count)*100/averageAllTime,
                };
        }
        private static async System.Threading.Tasks.Task<List<ToDoList>> GenerateListWithShowcase(TaskListSettingsViewModel listSetting, List<ListForDay> showcase)
        {
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
            var idealRatio = CalculateIdealRatio(showcase, userSettings);
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
            var blocks = new List<List<ToDoList>>();
            var additional = 0.0;
            int i = 0;
            foreach (var @group in applicantTaskGroups)
            {
                blocks.Add(GenerateBlocksFromGroups(@group, idealRatio[(int)@group.Key]+additional, userSettings));
                if (blocks[i][0].TimeEstimate.Ticks == 0)
                {
                    additional += idealRatio[(int)@group.Key];
                }
                else
                {
                    additional = 0.0;
                }
                i++;
            }
        //    var blocks = (from @group in applicantTaskGroups select GenerateBlocksFromGroups(@group, listSetting.TimeToFill.TotalMinutes * idealRatio[(int)@group.Key] / 100, userSettings)).ToList();
            return BuildToDoListsFromBlock(blocks, userSettings);
        }
        private static async System.Threading.Tasks.Task<List<ToDoList>> GenerateListWithIntuition(TaskListSettingsViewModel listSetting)
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
                .ToList();
      //      var blocks = (from @group in applicantTaskGroups select GenerateBlocksFromGroups(@group, listSetting.TimeToFill.TotalMinutes / applicantTaskGroups.Count, userSettings)).ToList();
            //var blocks = new List<List<ToDoList>>();
            //var minutEtalon = listSetting.TimeToFill.TotalMinutes / applicantTaskGroups.Count;
            //var totalMinuts = minutEtalon;
            //int i = 0;
            //foreach (var @group in applicantTaskGroups)
            //{
            //    blocks.Add(GenerateBlocksFromGroups(@group, totalMinuts, userSettings));
            //    if (blocks[i][0].TimeEstimate.Ticks == 0)
            //    {
            //        totalMinuts += minutEtalon;
            //    }
            //    else
            //    {
            //        totalMinuts = minutEtalon;
            //    }
            //    i++;
            //}
            var blocks = new List<ToDoList>[applicantTaskGroups.Count][];
            var minutEtalon = listSetting.TimeToFill.TotalMinutes / applicantTaskGroups.Count;
            for (int i = 0; i < applicantTaskGroups.Count; i++)
            {
                blocks[i] = new List<ToDoList>[applicantTaskGroups.Count];
                for (int j = 0; j < applicantTaskGroups.Count; j++)
                {
                    blocks[i][j] = GenerateBlocksFromGroups(applicantTaskGroups[i], minutEtalon*(j+1), userSettings);
                }
            }
            return BuildToDoListsFromBlock(blocks, userSettings);
        }
     
    }
}