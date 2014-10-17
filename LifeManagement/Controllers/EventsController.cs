using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using Microsoft.AspNet.Identity;
using Task = System.Threading.Tasks.Task;

namespace LifeManagement.Controllers
{
    [Authorize]
    [Localize]
    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var records = db.Records.Where(x => x.UserId == userId).OfType<Event>();
            return PartialView(await records.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var @event = await db.Records.FindAsync(id) as Event;
            if (@event == null)
            {
                return HttpNotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name");
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create");
            }
            return RedirectToAction("Index", "Cabinet");
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Note,StartDate,EndDate,IsUrgent")] Event @event)
        {
            var userId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                @event.Id = Guid.NewGuid();
                @event.UserId = userId;
                db.Records.Add(@event);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Cabinet");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var @event = await db.Records.FindAsync(id) as Event;
            if (@event == null)
            {
                return HttpNotFound();
            }

            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Note,StartDate,EndDate,IsUrgent")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var task = await db.Records.FindAsync(id) as Event;
            if (task == null)
            {
                return HttpNotFound();
            }

            return View(task);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var task = await db.Records.FindAsync(id);
            db.Records.Remove(task);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
