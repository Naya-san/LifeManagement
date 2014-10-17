using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Models;
using LifeManagement.Extensions;
using LifeManagement.Models.DB;
using Microsoft.AspNet.Identity;
using Task = LifeManagement.Models.DB.Task;

namespace LifeManagement.Controllers
{
    [Authorize]
    [Localize]
    public class TasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tasks
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().Include(t => t.Project).Include(t => t.Tags);
            return PartialView(await records.ToListAsync());
        }

        public async Task<ActionResult> Complete(Guid taskId)
        {
            var userId = User.Identity.GetUserId();
            var task = await db.Records.FirstOrDefaultAsync(x => x.Id == taskId && x.UserId == userId) as Task;

            if (task != null)
            {
                task.CompletedOn = DateTime.UtcNow;
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Cabinet");
        }

        // GET: Tasks/Details/5
        public async Task<ActionResult> Details(Guid? id)
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

            return View(task);
        }

        // GET: Tasks/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.ProjectId = new SelectList(db.Projects.Where(x => x.UserId == userId), "Id", "Path");
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name");
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create");
            }
            return RedirectToAction("Index", "Cabinet");
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Note,StartDate,EndDate,IsUrgent,ProjectId")] Task task)
        {
            var userId = User.Identity.GetUserId();
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (task.EndDate != null && Request["EndTime"] != "")
            {
                    var time = Request["EndTime"].Split(':');
                    var timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
                    task.EndDate = new DateTime(task.EndDate.Value.Ticks).Add(timeSpan);
            }
            task.EndDate = request.GetUtcFromUserLocalTime(task.EndDate);
            task.StartDate = request.GetUtcFromUserLocalTime(task.StartDate);
            string[] listTag = new[] {""};
            if (Request["Tags"] != "" && Request["Tag"] != null)
            {
                listTag = Request["Tags"].Split(',');
                foreach (string s in listTag)
                {
                    task.Tags.Add(await db.Tags.FindAsync(new Guid(s)));
                }
            }
            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid();
                task.UserId = userId;
                db.Records.Add(task);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Cabinet");
            }
            //ViewBag.Min = System.Web.HttpContext.Current.Request.GetUserLocalTimeFromUtc(DateTime.UtcNow.Date);
            ViewBag.ProjectId = new SelectList(db.Projects.Where(x => x.UserId == userId), "Id", "Path", task.ProjectId);
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name", listTag);
            return View(task);
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
            HttpRequest request = System.Web.HttpContext.Current.Request;
            task.EndDate = request.GetUserLocalTimeFromUtc(task.EndDate);
            task.StartDate = request.GetUserLocalTimeFromUtc(task.StartDate);
            ViewBag.Time = task.EndDate.toTimeFormat();
            if (Request.IsAjaxRequest())
            {
                return PartialView("Edit", task);
            }
            return RedirectToAction("Index", "Cabinet");
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Note,StartDate,EndDate,IsUrgent,ProjectId")] Task task)
        {
            if (task.EndDate != null && Request["EndTime"] != "")
            {
                var time = Request["EndTime"].Split(':');
                var timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
                task.EndDate = new DateTime(task.EndDate.Value.Ticks).Add(timeSpan);
            }

            HttpRequest request = System.Web.HttpContext.Current.Request;
            task.EndDate = request.GetUtcFromUserLocalTime(task.EndDate);
            task.StartDate = request.GetUtcFromUserLocalTime(task.StartDate);

            db.Records.Attach(task);
            
            if (Request["Tags"] != "" && Request["Tags"] != null)
            {
                foreach (var tag in task.Tags.ToList())
                {
                    task.Tags.Remove(tag);
                }

                var listTag = Request["Tags"].Split(',');
                foreach (string s in listTag)
                {
                    task.Tags.Add(await db.Tags.FindAsync(new Guid(s)));
                }
            }
            
            if (ModelState.IsValid)
            {
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Cabinet");
            }

            var userId = User.Identity.GetUserId();
            ViewBag.Time = task.EndDate.toTimeFormat();
            ViewBag.ProjectId = new SelectList(db.Projects.Where(x => x.UserId == userId), "Id", "Path", task.ProjectId);
            return View(task);
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
            return RedirectToAction("Index", "Cabinet");
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var task = await db.Records.FindAsync(id);
            db.Records.Remove(task);
            await db.SaveChangesAsync();
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
    }
}
