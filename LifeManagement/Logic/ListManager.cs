﻿using LifeManagement.Models.DB;
using System;
using System.Collections.Generic;
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
        public static List<Task> Generate(ApplicationDbContext db, TaskListSettingsViewModel listSetting)
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
            freeTime = (freeTime.Ticks < listSetting.TimeToFill.Ticks) ? freeTime : listSetting.TimeToFill;
            var showcase = db.ListsForDays.Where(x => x.UserId == listSetting.UserId && x.CompleteLevel >= SuccessLevel).ToList();
            if (showcase.Any())
            {

            }
            else
            {
                
            }

            return null;
        }

        private static List<Task> generateListWithShowcase(ApplicationDbContext db,
            TaskListSettingsViewModel listSetting, List<ListForDay> showcase)
        {
            var tasks= new List<Task>();
      //      showcase.Where(x=> x.Date.DayOfWeek == listSetting.Date.DayOfWeek && listSetting.TimeToFill == x.)
            return tasks;
        }
    }
}