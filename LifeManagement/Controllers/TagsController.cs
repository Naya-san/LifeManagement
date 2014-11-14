using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Extensions;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using Microsoft.AspNet.Identity;

namespace LifeManagement.Controllers
{
    [Authorize]
    [Localize]
    public class TagsController : ControllerExtensions
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tags
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var tags = db.Tags.Where(x => x.UserId == userId);
            return View(await tags.ToListAsync());
        }

        public async Task<ActionResult> GetRecords(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag tag = await db.Tags.FindAsync(id);
            if (tag == null)
            {
                return RedirectToAction("Index", "Cabinet");
            }
            
            return View(tag);
        }

        // GET: Tags/Create
        public ActionResult Create()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create");
            }
            return RedirectToPrevious();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                tag.Id = Guid.NewGuid();
                tag.UserId = User.Identity.GetUserId();
                db.Tags.Add(tag);
                await db.SaveChangesAsync();
                return RedirectToPrevious();
            }

            return View(tag);
        }

        // GET: Tags/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tag = await db.Tags.FindAsync(id);
            if (tag == null)
            {
                return RedirectToAction("Index", "Cabinet");
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("Edit", tag);
            }
            return RedirectToPrevious();
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                tag.UserId = User.Identity.GetUserId();
                db.Entry(tag).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Cabinet");
            }

            return PartialView("Edit", tag);
        }

        // GET: Tags/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tag = await db.Tags.FindAsync(id);
            if (tag == null)
            {
                return HttpNotFound();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Delete", tag);
            }
            return RedirectToPrevious();
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var tag = await db.Tags.FindAsync(id);
            db.Tags.Remove(tag);
            await db.SaveChangesAsync();
            return RedirectToPrevious();
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
