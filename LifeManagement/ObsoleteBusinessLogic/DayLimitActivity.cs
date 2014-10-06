using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LifeManagement.ObsoleteModels;

namespace LifeManagement.ObsoleteBusinessLogic
{
    public class DayLimitActivity
    {
        private LifeManagementContext db;

        public DayLimitActivity(LifeManagementContext _db)
        {
            db = _db;
        }
        public DayLimitActivity()
        {
            db = new LifeManagementContext();
        }

        public DayLimit CreatDefault(DateTime date, HttpRequest request, Guid user)
        {
            var beginDayLimit = new DateTime(date.Year, date.Month, date.Day, 6, 0, 0);
            var endDayLimit = beginDayLimit.AddHours(17);
            var dayLimit = new DayLimit()
            {
                Id = Guid.NewGuid(),
                UserId = user,
                IsDeleted = false,
                UpdatedOn = DateTime.UtcNow,
                StartDate = UserTimeConverter.GetUtcFromUserLocalTime(user, request, beginDayLimit),
                EndDate = UserTimeConverter.GetUtcFromUserLocalTime(user, request, endDayLimit)
            };

            if (IsConflicted(dayLimit))
            {
  
            }
            db.DayLimits.Add(dayLimit);
            db.SaveChanges();
            return dayLimit;
        }
        public DayLimit CreatDefault(DateTime startDate, DateTime endDate, Guid user,  HttpRequest request)
        {
            var beginDayLimit = UserTimeConverter.GetUtcFromUserLocalTime(user, request, new DateTime(startDate.Year, startDate.Month, startDate.Day, 6, 0, 0));
            var endDayLimit = UserTimeConverter.GetUtcFromUserLocalTime(user, request, new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 30, 0));
            var dayLimit = new DayLimit()
            {
                Id = Guid.NewGuid(),
                UserId = user,
                IsDeleted = false,
                UpdatedOn = DateTime.UtcNow,
                StartDate = (DateTime.Compare(beginDayLimit, startDate) <= 0) ? new DateTime(beginDayLimit.Ticks) : new DateTime(startDate.Ticks),
                EndDate = (DateTime.Compare(endDayLimit, endDate) >= 0) ? new DateTime(endDayLimit.Ticks) : new DateTime(endDate.Ticks),
            };

            
            if (IsConflicted(dayLimit))
            {
                dayLimit.StartDate = new DateTime(startDate.Ticks);
                dayLimit.EndDate = new DateTime(endDate.Ticks);
            }
            db.DayLimits.Add(dayLimit);
            db.SaveChanges();
            return dayLimit;
        }

        public DayLimit FindOrDefault(DateTime startDate, DateTime endDate, Guid userId,  HttpRequest request)
        {
            return (db.DayLimits.Any() ? db.DayLimits.FirstOrDefault(day => day.UserId.Equals(userId) && DateTime.Compare(day.StartDate, startDate) <= 0 && DateTime.Compare(day.EndDate, endDate) >= 0) : null) ??
                                 CreatDefault(startDate, endDate, userId, request);
        }
        public DayLimit FindOrDefault(DateTime date, Guid userId,  HttpRequest request)
        {
            return (db.DayLimits.Any() ? db.DayLimits.FirstOrDefault(day => day.UserId.Equals(userId) && day.StartDate.Day == date.Day 
                && day.StartDate.Month==date.Month && day.StartDate.Year==date.Year) : null) ??
                                 CreatDefault(date, request, userId);
        }
        public List<DayLimit> FindConflicts(DayLimit day)
        {
            return null;
        }

        public bool IsConflicted(DayLimit day)
        {
            List<DayLimit> conflicts = FindConflicts(day);
            if (conflicts != null && conflicts.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}