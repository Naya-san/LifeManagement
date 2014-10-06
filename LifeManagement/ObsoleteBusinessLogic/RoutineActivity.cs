using System;
using System.Linq;
using LifeManagement.ObsoleteModels;

namespace LifeManagement.ObsoleteBusinessLogic
{
    public class RoutineActivity
    {
        private const int MaxActivityLength = 30;
        private const int ActivityRest = 10;

        private readonly LifeManagementContext _context;
        private readonly DateTimeProvider _dateTimeProvider;

        public RoutineActivity(LifeManagementContext context, DateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public bool AddTaskToRoutine(DayLimit dayLimit, Task task, TimeSpan? maxActivityLength = null,
            TimeSpan? activityRest = null)
        {
            var now = _dateTimeProvider.UtcNow;
            if (dayLimit.EndDate < now)
            {
                return false;
            }

            var minimumStartDate = dayLimit.StartDate.Date == now.Date ? now.AddMinutes(1) : dayLimit.StartDate;
            maxActivityLength = maxActivityLength ?? TimeSpan.FromMinutes(MaxActivityLength);
            activityRest = activityRest ?? TimeSpan.FromMinutes(ActivityRest);
            maxActivityLength = maxActivityLength < task.Estimation ? maxActivityLength : task.Estimation;

            var routines = _context.Routines.Where(x => x.DayLimit.Id == dayLimit.Id && !x.IsDeleted && x.EndDate >= minimumStartDate).ToList();

            DateTime newStartDate, newEndDate;

            if (!routines.Any())
            {
                newStartDate = minimumStartDate;
                newEndDate = minimumStartDate.Add(maxActivityLength.Value);
            }
            else
            {
                routines = routines.OrderBy(x => x.StartDate).ToList();
                var lastRoutine = routines.Last();
                newStartDate = lastRoutine.EndDate.Add(activityRest.Value);
                newEndDate = newStartDate.Add(maxActivityLength.Value);

                if (newEndDate > dayLimit.EndDate)
                {
                    var freeTime = 0;

                    for (var i = 0; i < routines.Count - 1; i++)
                    {
                        var tempStartDate = routines[i].EndDate.Add(activityRest.Value);
                        var tempEndDate = routines[i + 1].StartDate.Add(-activityRest.Value);
                        var spaceBetweenRoutines = (tempEndDate - tempStartDate).TotalMinutes;

                        if (spaceBetweenRoutines <= freeTime) continue;

                        freeTime = (int) spaceBetweenRoutines;
                        newStartDate = tempStartDate;
                        newEndDate = tempEndDate;

                        if (freeTime >= maxActivityLength.Value.TotalMinutes)
                        {
                            break;
                        }
                    }
                }
            }

            if (newEndDate > dayLimit.EndDate) return false;

            var routine = new Routine
            {
                DayLimit = dayLimit,
                Id = Guid.NewGuid(),
                StartDate = newStartDate,
                EndDate = newEndDate,
                Type = Enums.RoutineType.Soft,
                Task = task,
                UserId = task.UserId,
                UpdatedOn = _dateTimeProvider.UtcNow
            };

            _context.Routines.Add(routine);
            _context.SaveChanges();
            return true;
        }
    }
}