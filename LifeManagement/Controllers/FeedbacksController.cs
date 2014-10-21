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
    public class FeedbacksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Feedbacks
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var feedbacks = db.Feedbacks.Where(x => x.UserId == userId);
            return View(await feedbacks.ToListAsync());
        }

        // GET: Feedbacks/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public ActionResult Create()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create");
            }
            return RedirectToAction("Index", "Cabinet");
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Subject,Message")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.Id = Guid.NewGuid();
                feedback.Date = DateTime.UtcNow;
                feedback.UserId = User.Identity.GetUserId();
                db.Feedbacks.Add(feedback);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Cabinet");
            }

            return View(feedback);
        }

        // GET: Feedbacks/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,Subject,Message")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.Date = DateTime.UtcNow;
                db.Entry(feedback).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var feedback = await db.Feedbacks.FindAsync(id);
            db.Feedbacks.Remove(feedback);
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
