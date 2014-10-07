using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using Microsoft.AspNet.Identity;

namespace LifeManagement.Controllers
{
    [Authorize]
    [Localize]
    public class AlertsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Alerts
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var alerts = db.Alerts.Where(x => x.UserId == userId).Include(a => a.Record);
            return View(await alerts.ToListAsync());
        }

        // GET: Alerts/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var alert = await db.Alerts.FindAsync(id);
            if (alert == null)
            {
                return HttpNotFound();
            }

            return View(alert);
        }

        // GET: Alerts/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.RecordId = new SelectList(db.Records.Where(x => x.UserId == userId), "Id", "Name");
            return View();
        }

        // POST: Alerts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RecordId,Name,Date")] Alert alert)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                alert.Id = Guid.NewGuid();
                alert.UserId = userId;
                db.Alerts.Add(alert);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.RecordId = new SelectList(db.Records.Where(x => x.UserId == userId), "Id", "Name", alert.RecordId);
            return View(alert);
        }

        // GET: Alerts/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var alert = await db.Alerts.FindAsync(id);
            if (alert == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            ViewBag.RecordId = new SelectList(db.Records.Where(x => x.UserId == userId), "Id", "Name", alert.RecordId);
            return View(alert);
        }

        // POST: Alerts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,RecordId,UserId,Name,Date")] Alert alert)
        {
            if (ModelState.IsValid)
            {
                db.Entry(alert).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var userId = User.Identity.GetUserId();
            ViewBag.RecordId = new SelectList(db.Records.Where(x => x.UserId == userId), "Id", "UserId", alert.RecordId);
            return View(alert);
        }

        // GET: Alerts/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var alert = await db.Alerts.FindAsync(id);
            if (alert == null)
            {
                return HttpNotFound();
            }

            return View(alert);
        }

        // POST: Alerts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var alert = await db.Alerts.FindAsync(id);
            db.Alerts.Remove(alert);
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
