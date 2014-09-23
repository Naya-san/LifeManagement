using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LifeManagement.Models;
using LifeManagement.Enums;
using LifeManagement.BusinessLogic;
using Microsoft.AspNet.Identity;
namespace LifeManagement.Controllers
{
    public class TaskController : Controller
    {
        private LifeManagementContext db = new LifeManagementContext();

        [Authorize]
        public ActionResult Complete(Guid taskId)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var task = db.Tasks.FirstOrDefault(x => x.Id == taskId && x.UserId == userId);

            if (task != null)
            {
                task.CompletedOn = DateTime.UtcNow;
                task.SpentTime = task.Estimation;
                task.UpdatedOn = DateTime.UtcNow;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Cabinet");
        }

        [Authorize]
        public ActionResult ToBackLog(Guid routineId)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var routine = db.Routines.FirstOrDefault(x => x.Id == routineId && x.UserId == userId);

            if (routine != null)
            {
                routine.IsDeleted = true;
                var now = DateTime.UtcNow;
                routine.UpdatedOn = now;
                if (routine.StartDate <= now && routine.EndDate >= now)
                {
                    var task = routine.Task;
                    task.SpentTime += TimeSpan.FromMinutes((now - routine.StartDate).TotalMinutes);
                    task.UpdatedOn = now;
                    if (task.SpentTime >= task.Estimation)
                    {
                        task.CompletedOn = now;
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Cabinet");
        }


        // GET: /Task/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = new Guid(User.Identity.GetUserId());
            Task task = db.Tasks.Find(id);

            if (task == null)
            {
                return HttpNotFound();
            }
           
            return View(task);
        }
        
        // GET: /Task/Create
        [Authorize]
        public ActionResult Create(Guid? projectId, Guid? parentTaskId)
        {           
           Guid userId = new Guid(User.Identity.GetUserId());
           Task ptask;
           if(parentTaskId != null && parentTaskId != Guid.Empty){
               ptask = db.Tasks.Find(parentTaskId);
               Session["container"] = "task";
           }
           else
           {
               ptask = db.Tasks.FirstOrDefault(p => p.UserId.Equals(userId) && !p.IsDeleted && p.CompletedOn == null);
               if (ptask != null)
               {
                 
                   Session["container"] = "project";
               }
               else
               {
                   ptask = new Task() { Id = Guid.Empty, Name = String.Empty};
                   Session["container"] = "projectOnly";
               }               
               
           }
            ViewBag.ParentTaskId = new SelectList(db.Tasks.Where(t => !t.IsDeleted && t.CompletedOn == null && t.UserId.Equals(ptask.UserId)), "Id", "Name", ptask.ParentTaskId);
            Project pProject;
            if (db.Projects.FirstOrDefault(x => x.UserId.Equals(userId)) == null)
            {
                db.Projects.Add(new Project
                {
                    Id = Guid.NewGuid(),
                    UpdatedOn = DateTime.UtcNow,
                    UserId = userId,
                    Name = Resources.ResourceScr.DefaultProject,
                });

                db.SaveChanges();
            }
            if (projectId != null && projectId != Guid.Empty)
            {
                pProject = db.Projects.Find(projectId);
            }
            else
            {
                pProject = db.Projects.FirstOrDefault(p => p.UserId.Equals(userId));
            }
            ViewBag.ProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(pProject.UserId)), "Id", "Path", pProject.Id);
            HttpRequest request = System.Web.HttpContext.Current.Request;
            var tmpDate = UserTimeConverter.GetUserLocalTimeFromUtc(userId, request, DateTime.UtcNow);
            tmpDate = tmpDate.AddMinutes(1);
            var defaultDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, tmpDate.Hour, tmpDate.Minute, 0);
            ViewBag.defaultStart = defaultDate.ToString("s");
            
            ViewBag.defaultEnd = defaultDate.AddMinutes(1).ToString("s");
            ViewBag.Priority = new SelectList(Enum.GetNames(typeof(Priority)));
            ViewBag.Complexity = new SelectList(Enum.GetNames(typeof(Complexity)));
            return View();
        }

        // POST: /Task/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Description,Priority,Complexity,Estimation,DesiredDueDate,Deadline,DesiredStartDate,CompletedOn,UserId,ProjectId,ParentTaskId")] Task task)
        {
            task.Priority = (Priority)Enum.Parse(typeof(Priority), Request["priorityChoice"], true);
            task.Complexity = (Complexity)Enum.Parse(typeof(Complexity), Request["complexityChoice"], true);
            task.UserId = new Guid(User.Identity.GetUserId());
            task.SpentTime = new TimeSpan(0, 0, 0);
            task.IsDeleted = false;
            task.UpdatedOn = DateTime.UtcNow;
            HttpRequest request = System.Web.HttpContext.Current.Request;
            Routine routine = null;
            if (Request["location"].Equals("soft"))
            {
                task.Estimation = new TimeSpan(Convert.ToInt32(Request["Days"]), Convert.ToInt32(Request["Hours"]), Convert.ToInt32(Request["Minutes"]), 0);
                if (Request["StartDate"]!=null)
                {
                    task.DesiredStartDate = UserTimeConverter.GetUtcFromUserLocalTime(task.UserId, request, DateTime.Parse(Request["StartDate"]));
                }
                if (Request["DueDate"]!=null)
                {
                    task.DesiredStartDate = UserTimeConverter.GetUtcFromUserLocalTime(task.UserId, request, DateTime.Parse(Request["DueDate"]));
                }
                if (Request["Deadlin"] != null)
                {
                    task.DesiredStartDate = UserTimeConverter.GetUtcFromUserLocalTime(task.UserId, request,DateTime.Parse(Request["Deadlin"]));
                }
            }
            else
            {
                routine = new Routine
                          {
                              Id = Guid.NewGuid(),
                              StartDate = UserTimeConverter.GetUtcFromUserLocalTime(task.UserId, request,DateTime.Parse(Request["Start"])),
                              EndDate = UserTimeConverter.GetUtcFromUserLocalTime(task.UserId, request,DateTime.Parse(Request["End"])),
                              IsDeleted = false,
                              UserId = task.UserId,
                              UpdatedOn = DateTime.UtcNow,
                              Type = RoutineType.Hard
                          };
                routine.DayLimitId = (new DayLimitActivity(db)).FindOrDefault(routine.StartDate, routine.EndDate, task.UserId,  System.Web.HttpContext.Current.Request).Id;
                task.Estimation = routine.EndDate.Subtract(routine.StartDate);
            }

            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid();
                if (task.ProjectId == Guid.Empty)
                {
                    Task  pTask = db.Tasks.Find(task.ParentTaskId);
                    task.ProjectId = pTask.ProjectId;
                }
                if (task.ParentTaskId == Guid.Empty)
                {
                    task.ParentTaskId = null;
                }
                db.Tasks.Add(task);

                if (routine != null)
                {
                    routine.TaskId = task.Id;
                    db.Routines.Add(routine);
                }
                db.SaveChanges();
                return RedirectToAction("Index", "Cabinet");
            }
            var dateTime = (routine != null) ? routine.StartDate : task.DesiredStartDate;
            if (dateTime != null)
            {
                DateTime defaultDate = (DateTime) dateTime;
                ViewBag.defaultStart = defaultDate.ToString("s");
                ViewBag.defaultEnd = defaultDate.AddTicks(task.EstimationTicks).ToString("s");
            }
            ViewBag.ParentTaskId = new SelectList(db.Tasks.Where(t => !t.IsDeleted && t.CompletedOn == null && t.UserId.Equals(task.UserId)), "Id", "Name", task.ParentTaskId);
            ViewBag.ProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(task.UserId)), "Id", "Path", task.ProjectId);
            return View(task);
        }


        // GET: /Task/Create
        [Authorize]
        public ActionResult QuickCreate(DateTime startDate, DateTime endDate)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            Guid userID = new Guid(User.Identity.GetUserId());
            var tmpTime = UserTimeConverter.GetUserLocalTimeFromUtc(userID, request, DateTime.UtcNow);
            if (startDate < tmpTime)
            {
                   startDate = new DateTime(tmpTime.Year, tmpTime.Month, tmpTime.Day, tmpTime.Hour, tmpTime.Minute, 0);
            }
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDate.Hour, endDate.Minute, 0);
            if (endDate < startDate)
            {
                TempData["alertMessage"] = Resources.ResourceScr.itsPast;
                return RedirectToAction("Index", "Cabinet");
            }
            ViewBag.StartDate = startDate.ToString("s");
            ViewBag.EndDate = endDate.ToString("s");
            ViewBag.StartDateMax = endDate.AddMinutes(-1).ToString("s");
            ViewBag.EndDateMin = startDate.AddMinutes(1).ToString("s");
            ViewBag.Priority = new SelectList(Enum.GetNames(typeof(Priority)));
            ViewBag.Complexity = new SelectList(Enum.GetNames(typeof(Complexity)));
            var userId = new Guid(User.Identity.GetUserId());
            ViewBag.ParentTaskId = new SelectList(db.Tasks.Where(t=>!t.IsDeleted && t.CompletedOn == null && t.UserId.Equals(userId)), "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(userId)), "Id", "Path");
            return View();
        }

        // POST: /Task/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult QuickCreate([Bind(Include = "Id,Name,Description,Priority,Complexity,ProjectId,ParentTaskId")] Task task)
        {
            task.Priority = (Priority)Enum.Parse(typeof(Priority), Request["priorityChoice"], true);
            task.Complexity = (Complexity)Enum.Parse(typeof(Complexity), Request["complexityChoice"], true);
            task.UserId = new Guid(User.Identity.GetUserId());
            task.SpentTime = new TimeSpan(0, 0, 0);
            task.IsDeleted = false;
            task.UpdatedOn = DateTime.UtcNow;
            task.Id = Guid.NewGuid();
            Routine routine  = new Routine
                {
                    Id = Guid.NewGuid(),
                    StartDate = UserTimeConverter.GetUtcFromUserLocalTime(task.UserId, System.Web.HttpContext.Current.Request, DateTime.Parse(Request["Start"])),
                    EndDate = UserTimeConverter.GetUtcFromUserLocalTime(task.UserId, System.Web.HttpContext.Current.Request, DateTime.Parse(Request["End"])),
                    IsDeleted = false,
                    UserId = task.UserId,
                    UpdatedOn = DateTime.UtcNow,
                    Type = RoutineType.Hard,
                    TaskId = task.Id
                };
            routine.DayLimitId = (new DayLimitActivity(db)).FindOrDefault(routine.StartDate, routine.EndDate, task.UserId, System.Web.HttpContext.Current.Request).Id;
            task.Estimation = routine.EndDate.Subtract(routine.StartDate);

            if (ModelState.IsValid)
            {
                if (task.ProjectId == Guid.Empty)
                {
                    Task pTask = db.Tasks.Find(task.ParentTaskId);
                    task.ProjectId = pTask.ProjectId;
                }
                if (task.ParentTaskId == Guid.Empty)
                {
                    task.ParentTaskId = null;
                }
               
                db.Tasks.Add(task);
                db.Routines.Add(routine);
                db.SaveChanges();
                return RedirectToAction("Index", "Cabinet");
            }

            ViewBag.StartDate = Request["Start"];
            ViewBag.EndDate = Request["End"];
            ViewBag.StartDateMax = DateTime.Parse(Request["End"]).AddMinutes(-1).ToString("s");
            ViewBag.EndDateMin = DateTime.Parse(Request["Start"]).AddMinutes(1).ToString("s");
            ViewBag.ParentTaskId = new SelectList(db.Tasks.Where(t => !t.IsDeleted && t.CompletedOn == null && t.UserId.Equals(task.UserId)), "Id", "Name", task.ParentTaskId);
            ViewBag.ProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(task.UserId)), "Id", "Name", task.ProjectId);
            return View(task);
        }

        // GET: /Task/Edit/5
         [Authorize]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            Guid userId = new Guid(User.Identity.GetUserId());
            if (task == null || task.IsDeleted || !task.UserId.Equals(userId))
            {
                return HttpNotFound();
            }
             if (!db.Tasks.Any(x => x.UserId.Equals(userId) && !x.IsDeleted && !x.Id.Equals(task.Id)))
             {
                 Session["container"] = "projectOnly";
             }
             else
             {
                 if (task.ParentTaskId == null)
                 {
                     Session["container"] = "project";
                 }
                 else
                 {
                     Session["container"] = "task";
                 }
             }
            
            ViewBag.Priority = new SelectList(Enum.GetNames(typeof(Priority)));
            ViewBag.Complexity = new SelectList(Enum.GetNames(typeof(Complexity)));
            ViewBag.ParentTaskId = new SelectList(db.Tasks.Where(t => !t.IsDeleted && t.CompletedOn == null && t.UserId.Equals(task.UserId) && !t.Id.Equals(task.Id)), "Id", "Name", task.ParentTaskId);
            ViewBag.ProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(task.UserId)), "Id", "Name", task.ProjectId);
            return View(task);
        }

        // POST: /Task/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,UpdatedOn,IsDeleted,Name,Description,Priority,Complexity,Estimation,SpentTime,DesiredDueDate,Deadline,DesiredStartDate,CompletedOn,UserId,ProjectId,ParentTaskId")] Task task)
        {
            task.UpdatedOn = DateTime.UtcNow;
            if (task.ParentTaskId != null)
            {
                task.ParentTask = db.Tasks.Find(task.ParentTaskId);
                if (task.ParentTask != null && !task.ProjectId.Equals(task.ParentTask.ProjectId))
                {
                    task.ProjectId = task.ParentTask.ProjectId;
                }
            }
            if (ModelState.IsValid)
            {
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Cabinet");
            }
            ViewBag.ParentTaskId = new SelectList(db.Tasks, "Id", "Name", task.ParentTaskId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Path", task.ProjectId);
            return View(task);
        }

        // GET: /Task/Delete/5
         [Authorize]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
             Guid userId = new Guid(User.Identity.GetUserId());
            if (task == null || task.IsDeleted || !task.UserId.Equals(userId))
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: /Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Task task = db.Tasks.Find(id);
            task.Delite(db);
            db.SaveChanges();
            return RedirectToAction("Index", "Cabinet");
        }

        public ActionResult AddToRoutine(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            Guid userId = new Guid(User.Identity.GetUserId());
            if (task == null || task.IsDeleted || !task.UserId.Equals(userId))
            {
                return HttpNotFound();
            }
            var routineActivity = new RoutineActivity(db, new DateTimeProvider());

            if (!routineActivity.AddTaskToRoutine((new DayLimitActivity(db)).FindOrDefault(DateTime.UtcNow, task.UserId, System.Web.HttpContext.Current.Request), task))
            {
                TempData["alertMessage"] = Resources.ResourceScr.cantAddToList;
            }
            return RedirectToAction("Index", "Cabinet");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize]
        public ActionResult NothingToDo(DateTime startDate, DateTime endDate)
        {
            Guid userId = new Guid(User.Identity.GetUserId());
            HttpRequest request = System.Web.HttpContext.Current.Request;
            DateTime end = UserTimeConverter.GetUtcFromUserLocalTime(userId, request, endDate);
            DateTime start;
            var tmpTime = UserTimeConverter.GetUserLocalTimeFromUtc(userId, request, DateTime.UtcNow);
            if (startDate > tmpTime)
            {
                start = UserTimeConverter.GetUtcFromUserLocalTime(userId, request, startDate);
            }
            else
            {
               start = DateTime.UtcNow;
            }
            if (start > end)
            {
                TempData["alertMessage"] = Resources.ResourceScr.itsPast;
                return RedirectToAction("Index", "Cabinet");
            }
            DayLimit dayLimit = (new DayLimitActivity(db)).FindOrDefault(start, end, userId, request);
            var tasks = db.Tasks.Where(x => x.UserId.Equals(userId) && !x.IsDeleted).OrderBy(x => x.Priority).ToList();
            if (dayLimit.Routines != null && tasks != null)
            {
                tasks = tasks.Where(t => !dayLimit.Routines.Any(r => r.TaskId.Equals(t.Id) && !r.IsDeleted)).ToList();
            }
            if (tasks != null && tasks.Any())
            {
                var task = tasks[tasks.Count - 1];
                if (start.Add(task.Estimation.Subtract(task.SpentTime)) < end)
                {
                    end = start.Add(task.Estimation.Subtract(task.SpentTime));
                }
                Routine routine = new Routine
                {
                    Id = Guid.NewGuid(),
                    StartDate = start,
                    EndDate = end,
                    IsDeleted = false,
                    UserId = userId,
                    UpdatedOn = DateTime.UtcNow,
                    Type = RoutineType.Soft,
                    TaskId = task.Id
                };
                routine.DayLimitId = dayLimit.Id;
                db.Routines.Add(routine);
                db.SaveChanges();
            }
            else
            {
                TempData["alertMessage"] = Resources.ResourceScr.noAvailableTasks;
            }
            return RedirectToAction("Index", "Cabinet");
        }

    }
}
