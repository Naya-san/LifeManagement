using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using LifeManagement.Attributes;
using LifeManagement.Enums;
using LifeManagement.Hubs;
using LifeManagement.Models;
using LifeManagement.Extensions;
using LifeManagement.Models.DB;
using LifeManagement.ViewModels;
using LifeManagement.Resources;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Task = LifeManagement.Models.DB.Task;

namespace LifeManagement.Controllers
{
    [System.Web.Mvc.Authorize]
    [Localize]
    public class TasksController : ControllerExtensions
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult FilterTasks(RecordFilter recordFilter)
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().Where(x => x.CompletedOn == null).ToList();
            records = FilterRecords(records, recordFilter);
            ConvertTasksToUserLocalTime(request, records);
            return PartialView("Index", records);
        }

        public ActionResult GetTasksCompleatedToday()
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
             var today = DateTime.UtcNow.Date;
             var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().Where(x => x.CompletedOn.HasValue && x.CompletedOn.Value.Year== today.Year && x.CompletedOn.Value.Month == today.Month && x.CompletedOn.Value.Day == today.Day).ToList();
            ConvertTasksToUserLocalTime(request, records);
            return PartialView("Index", records);
        }
        public ActionResult GetTasksCompleatedWeek()
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var today = DateTime.UtcNow.Date;
            var week = today.AddDays(-7);
            var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().Where(x => x.CompletedOn.HasValue &&
                x.CompletedOn.Value.Year <= today.Year && x.CompletedOn.Value.Month <= today.Month && x.CompletedOn.Value.Day <= today.Day &&
                x.CompletedOn.Value.Year >= week.Year && x.CompletedOn.Value.Month >= week.Month && x.CompletedOn.Value.Day >= week.Day).ToList();
            ConvertTasksToUserLocalTime(request, records);
            return PartialView("Index", records);
        }
        public ActionResult GetTasksByProject(Guid projectId)
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().Where(x => x.CompletedOn == null && x.ProjectId == projectId).ToList();
            ConvertTasksToUserLocalTime(request, records);
            return PartialView("Index", records);
        }

        public ActionResult GetTasksByTag(Guid tagId)
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().Where(x => x.CompletedOn == null).Include(x => x.Tags).Where(x => x.Tags.Any(y => y.Id == tagId)).ToList();
            ConvertTasksToUserLocalTime(request, records);
            return PartialView("Index", records);
        }

        public ActionResult GetTasksByText(string text)
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().ToList().Where(x => x.Name.ToLower().Contains(text.ToLower()));
            ConvertTasksToUserLocalTime(request, records);
            return PartialView("Index", records);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCompleteLevel(string id, byte level)
        {
            Guid taskId;
            Guid.TryParse(id, out taskId);
            if (taskId == null)
            {
                return Json(new { success = -1, time = "" });
            }

            var task = await db.Records.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == taskId) as Task;
            if (task == null)
            {
                return Json(new { success = -1, time = "" });
            }
            if(task.CompleteLevel == level){
                return Json(new { success = 0, time = "" });
            }
            int res = 1;
            if (level < 100 && task.CompleteLevel == 100)
            {
                task.CompletedOn = null;
                res = 3;
            }
            task.CompleteLevel = level;
            if (task.CompleteLevel == 100)
            {
                res = 2;
                task.CompletedOn = DateTime.UtcNow;
            }
            if (ModelState.IsValid)
            {
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var request = System.Web.HttpContext.Current.Request;
                ConvertTaskToUserLocalTime(request, task);
                return Json(new { success = res, time = task.ConvertTimeToNice(true)});
            }

            return Json(new { success = -1, time = ""});
        }
        // GET: Tasks
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().ToList();
            ConvertTasksToUserLocalTime(request, records);
            return PartialView(records);
        }

        public ActionResult NothingToDoTask()
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var today = DateTime.UtcNow.Date.AddMinutes(1439);
            var dueDate = today.AddDays(7);
            var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().Where(x => !x.CompletedOn.HasValue &&
                ((!x.StartDate.HasValue && !x.EndDate.HasValue) ||
                (x.EndDate != null && x.EndDate <= dueDate && x.EndDate > today && (x.StartDate == null || (x.StartDate != null && x.StartDate > today))))).OrderBy(x => !x.IsImportant).ToList();
            ConvertTasksToUserLocalTime(request, records);
            return PartialView(records);
        }

        [HttpPost]
        public ActionResult MakeTask(Guid[] TasksId)
        {
            if (TasksId == null)
            {
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            var today = DateTime.UtcNow.Date;
            foreach (var guid in TasksId)
            {
                var task = db.Records.Find(guid);
                if (task != null)
                {
                    task.StartDate = today;
                    db.Entry(task).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        public ActionResult GenerateList()
        {
            var settings = new TaskListSettingsViewModel();
            ViewBag.Date = DateTime.Now.Date.ToString("dd.MM.yyyy");
            ViewBag.Time = "00:05";

            return PartialView(settings);
        }

        [HttpPost]
        public async Task<ActionResult> GenerateList([Bind(Include = "Date,TimeToFill")]TaskListSettingsViewModel listSetting)
        {
            if (listSetting.Date < DateTime.UtcNow.Date)
            {
                ViewBag.Date = listSetting.Date.ToString("dd.MM.yyyy");
                ViewBag.Time = listSetting.TimeToFill.Hours + ":" + listSetting.TimeToFill.Minutes;
                ModelState.AddModelError("Date", ResourceScr.itsPast);
                return PartialView("GenerateList", listSetting);
            }
            var listForDay = db.FirstOrDefaultListTaskAsync(listSetting.Date);

            return Json(new { success = true });
        }

        public void SendAlertToClient(Guid alertId, DateTime userLocalTime)
        {
            var alert = db.Alerts.FirstOrDefault(x => x.Id == alertId);
            if (alert == null) return;

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<AlertsHub>();
            hubContext.Clients.User(alert.User.UserName).addAlert(new AlertViewModel { Id = alert.Id, Name = alert.Name, Date = userLocalTime.ToString() });
        }

        private bool CheckComplexity(Task task, ModelStateDictionary modelState)
        {
            bool res = true;
            if (task.StartDate.HasValue && task.EndDate.HasValue && task.Complexity != Complexity.None)
            {
                var settings = db.UserSettings.FirstOrDefaultAsync(x => x.UserId == task.UserId).Result ?? new UserSetting();
                res = task.EndDate.Value.Subtract(task.StartDate.Value).Ticks >
                       settings.GetMinComplexityRange(task.Complexity).Ticks;
                if (!res)
                {
                    modelState.AddModelError("Complexity", ResourceScr.ErrorComplexity);
                }

            }
            return res;
        }
        // GET: Tasks/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            if (!db.Projects.Any(x => x.UserId == userId))
            {
                var project = new Project { Name =ResourceScr.DefaultProject, Id = Guid.NewGuid(), UserId = userId };

                if (ModelState.IsValid)
                {
                    db.Projects.Add(project);
                    db.SaveChanges();
                }
            }
            ViewBag.ProjectId = new SelectList(db.Projects.Where(x => x.UserId == userId), "Id", "Path");
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name");
            ViewBag.Alerts = AlertPosition.None.ToSelectList();
            ViewBag.Complexity = Complexity.None.ToSelectList();
            ViewBag.StartDate = "";
            ViewBag.EndDate = "";
            ViewBag.Time = "";
            ViewBag.BlockState = "closeBlock";
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create");
            }
            return RedirectToPrevious();
        }
        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Note,StartDate,EndDate,IsImportant,ProjectId,Complexity")] Task task)
        {
            var userId = User.Identity.GetUserId();
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (task.EndDate.HasValue && Request["EndTime"] != "")
            {
                    var time = Request["EndTime"].Split(':');
                    var timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
                    task.EndDate = new DateTime(task.EndDate.Value.Ticks).Add(timeSpan);
            }
            string startString = task.StartDate.HasValue? task.StartDate.Value.ToString("g") : task.EndDate.HasValue? task.EndDate.Value.ToString("g") : "";
            task.EndDate = request.GetUtcFromUserLocalTime(task.EndDate);
            task.StartDate = request.GetUtcFromUserLocalTime(task.StartDate);
            task.CompleteLevel = 0;
            string[] listTag = new[] {""};
            if (Request["Tags"] != "" && Request["Tags"] != null)
            {
                listTag = Request["Tags"].Split(',');
                foreach (string s in listTag)
                {
                    task.Tags.Add(await db.Tags.FindAsync(new Guid(s)));
                }
            }
            int alertPosition = Int32.MinValue;
            if (Request["Alerts"] != null)
            {
                alertPosition = Convert.ToInt32(Request["Alerts"]);
            }
            task.UserId = userId;
            if (ModelState.IsValid && task.IsTimeValid(ModelState, AlertPosition.None.Parse(alertPosition)) && CheckComplexity(task,ModelState))
            {
                task.Id = Guid.NewGuid();
                Alert alert = null;

                if (alertPosition >= 0)
                {
                    alert = new Alert { UserId = userId, Position = (AlertPosition)alertPosition, RecordId = task.Id, Id = Guid.NewGuid(), Date = (task.StartDate.HasValue) ? task.StartDate.Value.AddMinutes(-1 * alertPosition) : task.EndDate.Value.AddMinutes(-1 * alertPosition), Name = String.Concat(task.Name, Resources.ResourceScr.at, startString)};
                    db.Alerts.Add(alert);
                }
                db.Records.Add(task);
                await db.SaveChangesAsync();

                if (alert == null)
                {
                    return Json(new { success = true });
                }

                var timespan = alert.Date - DateTime.UtcNow;
                if (timespan.TotalSeconds > 0)
                {
                    BackgroundJob.Schedule(() => SendAlertToClient(alert.Id, request.GetUserLocalTimeFromUtc(alert.Date)), timespan);
                }

                return Json(new { success = true });
            }
            ViewBag.ProjectId = new SelectList(db.Projects.Where(x => x.UserId == userId), "Id", "Path", task.ProjectId);
            ViewBag.Alerts = AlertPosition.None.Parse(alertPosition).ToSelectList();
            ViewBag.Complexity = task.Complexity.ToSelectList();
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name", listTag);
            ViewBag.StartDate = task.StartDate.HasValue ? request.GetUserLocalTimeFromUtc(task.StartDate).Value.ToString("dd.MM.yyyy") : "";
            ViewBag.EndDate = task.EndDate.HasValue ? request.GetUserLocalTimeFromUtc(task.EndDate).Value.ToString("dd.MM.yyyy") : "";
            ViewBag.Time = Request["EndTime"]!=""? Request["EndTime"]: "00:00";
            ViewBag.BlockState = "openBlock";
            return PartialView("Create", task);
        }
        
        public async Task<ActionResult> Display(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var task = await db.Records.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id) as Task;
            var userId = User.Identity.GetUserId();
            if (task == null || task.UserId != userId)
            {
                return HttpNotFound();
            }

            var alert = await db.Alerts.FirstOrDefaultAsync(x => x.UserId == userId && x.RecordId == task.Id);
            ViewBag.Alerts = alert == null ? AlertPosition.None.ToSelectList() : alert.Position.ToSelectList();
            HttpRequest request = System.Web.HttpContext.Current.Request;
            ConvertTaskToUserLocalTime(request, task);
            
            if (task.CompleteLevel >= 100)
            {
                return PartialView("Display", task);
            }
            ViewBag.Time = task.EndDate.ToTimeFormat();
            ViewBag.ProjectId = new SelectList(db.Projects.Where(x => x.UserId == userId), "Id", "Path", task.ProjectId);
            ViewBag.Complexity = task.Complexity.ToSelectList();
            return PartialView("Edit", task);
        }
        // GET: Tasks/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var task = await db.Records.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id) as Task;
            if (task == null)
            {
                return HttpNotFound();
            }
            HttpRequest request = System.Web.HttpContext.Current.Request;
            ConvertTaskToUserLocalTime(request, task);
            ViewBag.Time = task.EndDate.ToTimeFormat();
            if (task.CompleteLevel >= 100)
            {
                return PartialView("Display", task);
            }
            var userId = User.Identity.GetUserId();
            ViewBag.ProjectId = new SelectList(db.Projects.Where(x => x.UserId == userId), "Id", "Path", task.ProjectId);
            var selected = new string[task.Tags.Count];
            int i = 0;
            foreach (var tag in task.Tags)
            {
                selected[i] = tag.Id.ToString();
                i++;
            }
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name", selected);
            var alert = await db.Alerts.FirstOrDefaultAsync(x => x.UserId == userId && x.RecordId == task.Id);
            ViewBag.Alerts = alert == null ? AlertPosition.None.ToSelectList() : alert.Position.ToSelectList();
            ViewBag.Complexity = task.Complexity.ToSelectList();
            if (Request.IsAjaxRequest())
            {
                return PartialView("Edit", task);
            }
            return RedirectToPrevious();
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Note,StartDate,EndDate,IsImportant,ProjectId,Complexity,CompleteLevel,CompletedOn")] Task task)
        {
            if (task.EndDate != null && Request["EndTime"] != "")
            {
                var time = Request["EndTime"].Split(':');
                var timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
                task.EndDate = new DateTime(task.EndDate.Value.Ticks).Add(timeSpan);
            }
            string startString = task.StartDate.HasValue ? task.StartDate.Value.ToString("g") : task.EndDate.HasValue ? task.EndDate.Value.ToString("g") : "";
            HttpRequest request = System.Web.HttpContext.Current.Request;
            task.EndDate = request.GetUtcFromUserLocalTime(task.EndDate);
            task.StartDate = request.GetUtcFromUserLocalTime(task.StartDate);

            db.Records.Attach(task);
            db.Entry(task).Collection(x => x.Tags).Load();
            db.Entry(task).Collection(x => x.Alerts).Load();
            if (Request["Tags"] != "" && Request["Tags"] != null)
            {
                List<Tag> newTags = new List<Tag>();
                var tags = Request["Tags"].Split(',');
                foreach (string s in tags)
                {
                    newTags.Add(await db.Tags.FindAsync(new Guid(s)));
                }
                List<Tag> newTagsTmp = task.Tags.ToList();
                foreach (var tag in newTagsTmp)
                {
                    if (!newTags.Contains(tag))
            {
                task.Tags.Remove(tag);
            }
                }
                foreach (var tag in newTags)
            {
                    if (!task.Tags.Contains(tag))
                {
                        task.Tags.Add(tag);
                    }
                }
            }


            int alertPosition = Int32.MinValue;
            if (Request["Alerts"] != null)
            {
                alertPosition = Convert.ToInt32(Request["Alerts"]);
            }
            var date = task.StartDate.HasValue ? task.StartDate.Value : task.EndDate;
            Alert alert = (task.Alerts != null && task.Alerts.Any())? task.Alerts.ToArray()[0]: null;
            if (alertPosition >= 0 && date != null && alert != null)
            {
                if (alertPosition != (int)alert.Position || (date.Value - alert.Date).TotalMinutes != alertPosition)
                {
                    alert.Name = String.Concat(task.Name, ResourceScr.at, startString);
                    alert.Date = date.Value.AddMinutes(-1*alertPosition);
                    alert.Position = (AlertPosition)alertPosition;
                }
            }
            else
            {
                if (alertPosition >= 0 && date != null && alert == null)
                {
                    alert = new Alert { UserId = task.UserId, Position = (AlertPosition)alertPosition, RecordId = task.Id, Id = Guid.NewGuid(), Date = (task.StartDate.HasValue) ? task.StartDate.Value.AddMinutes(-1 * alertPosition) : task.EndDate.Value.AddMinutes(-1 * alertPosition), Name = String.Concat(task.Name, ResourceScr.at, startString) };
                    db.Alerts.Add(alert);
                }
                else
                {
                    if ((alertPosition < 0 || date == null) && alert != null)
                    {
                        db.Alerts.Remove(alert);
                    }
                }
            }

            if (ModelState.IsValid && task.IsTimeValid(ModelState, AlertPosition.None.Parse(alertPosition)) && CheckComplexity(task, ModelState))
            {
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            var userId = User.Identity.GetUserId();
            ViewBag.Time = task.EndDate.ToTimeFormat();
            var selected = new string[task.Tags.Count];
            int i = 0;
            foreach (var tag in task.Tags)
            {
                selected[i] = tag.Id.ToString();
                i++;
            }
            ViewBag.Alerts = AlertPosition.None.Parse(alertPosition).ToSelectList();
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name", selected);
            ViewBag.ProjectId = new SelectList(db.Projects.Where(x => x.UserId == userId), "Id", "Path", task.ProjectId);
            task.EndDate = request.GetUserLocalTimeFromUtc(task.EndDate);
            task.StartDate = request.GetUserLocalTimeFromUtc(task.StartDate);
            ViewBag.Complexity = task.Complexity.ToSelectList();
            ViewBag.Time = task.EndDate.ToTimeFormat();
            return PartialView("Edit", task);
        }

        // GET: Tasks/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var task = await db.Records.FindAsync(id) as Task;
            if (task == null)
            {
                return HttpNotFound();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Delete", task);
            }
            return RedirectToPrevious();
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var task = await db.Records.FindAsync(id);
            db.Records.Remove(task);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        protected void ConvertTaskToUserLocalTime(HttpRequest request, Task record)
        {
            record.StartDate = request.GetUserLocalTimeFromUtc(record.StartDate);
            record.EndDate = request.GetUserLocalTimeFromUtc(record.EndDate);
            record.CompletedOn = request.GetUserLocalTimeFromUtc(record.CompletedOn);            
        }
        protected void ConvertTasksToUserLocalTime(HttpRequest request, IEnumerable<Task> records)
        {
            foreach (var record in records)
            {
                ConvertTaskToUserLocalTime(request, record);
            }
        }

        protected List<Task> FilterRecords(List<Task> records, RecordFilter recordFilter)
        {
            var today = DateTime.UtcNow.Date;
            switch (recordFilter)
            {
                case RecordFilter.Today:
                    {
                        var dueDate = DateTime.UtcNow.Date;
                        return
                            records.Where(
                                x =>
                                    (x.StartDate != null && x.StartDate.Value.Date <= dueDate && ((x.EndDate.HasValue && x.EndDate.Value >= dueDate) || !x.EndDate.HasValue)) ||
                                    (x.EndDate != null && x.EndDate.Value.Date == dueDate && !x.StartDate.HasValue)).ToList();
                    }
                case RecordFilter.Overdue:
                    return
                        records.Where(
                            x =>
                                (x.EndDate != null && x.EndDate.Value.Date < DateTime.UtcNow.Date)).ToList();
                case RecordFilter.Tomorrow:
                    {
                        var dueDate = DateTime.UtcNow.Date.AddDays(1);
                        return
                            records.Where(
                                x =>
                                    (x.StartDate != null && x.StartDate.Value.Date == dueDate && (x.EndDate == null || (x.EndDate != null && x.EndDate > today))) ||
                                    (x.EndDate != null && x.EndDate.Value.Date == dueDate)).ToList();
                    }
                case RecordFilter.Future:
                    {
                        var dueDate = DateTime.UtcNow.Date;
                        return
                            records.Where(
                                x =>
                                    (x.StartDate != null && x.StartDate.Value.Date > dueDate) ||
                                    (x.EndDate != null && x.EndDate.Value.Date > dueDate && (x.StartDate == null || (x.StartDate != null && x.StartDate > today)))).ToList();
                    }
                case RecordFilter.NoDueDate:
                    return
                        records.Where(
                            x =>
                                (x.StartDate == null && x.EndDate == null)).ToList();
                default:
                    return records;
            }
        } 
    }
}
