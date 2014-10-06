using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using Microsoft.AspNet.Identity;

namespace LifeManagement.Controllers
{
    [Authorize]
    [Localize]
    public class TagsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tags
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var tags = db.Tags.Where(x => x.UserId == userId).Include(t => t.User);
            return View(await tags.ToListAsync());
        }

        // GET: Tags/Details/5
        public async Task<ActionResult> Details(Guid? id)
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

            return View(tag);
        }

        // GET: Tags/Create
        public ActionResult Create()
        {
            return View();
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
                return RedirectToAction("Index");
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
                return HttpNotFound();
            }

            return View(tag);
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
                return RedirectToAction("Index");
            }

            return View(tag);
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

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var tag = await db.Tags.FindAsync(id);
#warning Нужно удалять теги у всех записей, у которых они есть. Могут быть проблемы с каскадами.
            db.Tags.Remove(tag);
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
