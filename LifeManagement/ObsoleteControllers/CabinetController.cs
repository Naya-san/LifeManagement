using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Enums;
using LifeManagement.ObsoleteBusinessLogic;
using LifeManagement.ObsoleteModels;
using Microsoft.AspNet.Identity;

namespace LifeManagement.ObsoleteControllers
{
    [Localize]
    public class CabinetController : Controller
    {
        private LifeManagementContext db = new LifeManagementContext();
        //
        // GET: /Cabinet/
        [Authorize]
        public ActionResult Index()
        {
            const int activityRest = 10;

            var userId = Guid.Parse(User.Identity.GetUserId());
            var dateTime = DateTime.UtcNow;

            var dayLimit = db.DayLimits.FirstOrDefault(x => x.UserId == userId &&  DateTime.Compare(x.StartDate, dateTime) <= 0 && DateTime.Compare(x.EndDate,dateTime) >= 0 && !x.IsDeleted);
            var routines = new List<Routine>();
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (dayLimit == null)
            {
                DayLimitActivity dayLimitActivity = new DayLimitActivity(db);
                dayLimit = dayLimitActivity.CreatDefault(dateTime, request,  userId);
            }
            var temp = dayLimit.Routines == null ? null : dayLimit.Routines.Where(x => !x.IsDeleted).OrderBy(x => x.StartDate).ToList();
                if (temp == null || !temp.Any())
                {
                    routines.Add(new Routine {Type = RoutineType.Free, StartDate = dayLimit.StartDate, EndDate = dayLimit.EndDate});
                }
                else
                {
                    if ((temp[0].StartDate - dayLimit.StartDate).TotalMinutes > activityRest)
                    {
                        routines.Add(new Routine {Type = RoutineType.Free, StartDate = dayLimit.StartDate, EndDate = temp[0].StartDate});
                    }

                    for (var i = 0; i < temp.Count - 1; i++)
                    {
                        routines.Add(temp[i]);
                        if ((temp[i + 1].StartDate - temp[i].EndDate).TotalMinutes > activityRest)
                        {
                            routines.Add(new Routine { Type = RoutineType.Free, StartDate = temp[i].EndDate, EndDate = temp[i+1].StartDate });
                        }
                    }

                    routines.Add(temp.Last());
                    
                    if ((dayLimit.EndDate - temp.Last().EndDate).TotalMinutes > activityRest)
                    {
                        routines.Add(new Routine {Type = RoutineType.Free, StartDate = temp.Last().EndDate, EndDate = dayLimit.EndDate});
                    }
                }

            foreach (var routine in routines)
            {
                routine.StartDate = UserTimeConverter.GetUserLocalTimeFromUtc(userId, request, routine.StartDate);
                routine.EndDate = UserTimeConverter.GetUserLocalTimeFromUtc(userId, request, routine.EndDate);
            }

            return View(routines);
        }

        [Authorize]
        public ActionResult Sidebar()
        {
            Guid userId = new Guid(User.Identity.GetUserId());
            var projects = db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(userId) && p.ParentProjectId.Equals(null)).Include(p => p.Tasks).Include(p => p.ChildProjects);
            return View(projects.ToList());
        }
	}
}