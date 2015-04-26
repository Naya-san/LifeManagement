using LifeManagement.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LifeManagement.Enums;
using LifeManagement.Models;
using LifeManagement.ViewModels;

namespace LifeManagement.Logic
{
    public static class ListManager
    {
        public static List<ListForDay> Generate(ApplicationDbContext db, TaskListSettingsViewModel listSetting, string userId)
        {
            var recordsOnDate =
                db.Records.Where(x => x.UserId == userId &&
                    (((x.StartDate.HasValue && x.StartDate.Value.Date <= listSetting.Date) && ((x.EndDate.HasValue && x.EndDate.Value.Date >= listSetting.Date) || !x.EndDate.HasValue))
                    ||
                    (!x.StartDate.HasValue && x.EndDate.HasValue && x.EndDate.Value.Date >= listSetting.Date))).ToList();
            var settings = db.UserSettings.FirstOrDefault(x => x.UserId == userId) ?? new UserSetting(userId);
            var ticksInUse = settings.WorkingTime.Subtract(new TimeSpan(recordsOnDate.Sum(record => record.CalculateTimeLeft(settings).Ticks)));

            //if (listSetting.TimeToFill.Subtract(ticksInUse) > settings.GetMinComplexityRange(Complexity.Low))
            //{
            //    throw new Exception();
            //}
            return null;
        }
    }
}