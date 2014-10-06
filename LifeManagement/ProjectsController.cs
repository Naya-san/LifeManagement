using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LifeManagement.Models;
using LifeManagement.Models.DB;

namespace LifeManagement
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public async Task<ActionResult> Index()
        {
            db.Projects.Add(new Project {Name = "test", UserId = Guid.NewGuid().ToString()});
            db.SaveChanges();
            var projects = db.Projects.Include(p => p.ParentProject).Include(p => p.User);
            return View(await projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.ParentProjectId = new SelectList(db.Projects, "Id", "UserId");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ParentProjectId,UserId,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Id = Guid.NewGuid();
                db.Projects.Add(project);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ParentProjectId = new SelectList(db.Projects, "Id", "UserId", project.ParentProjectId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", project.UserId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentProjectId = new SelectList(db.Projects, "Id", "UserId", project.ParentProjectId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", project.UserId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ParentProjectId,UserId,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ParentProjectId = new SelectList(db.Projects, "Id", "UserId", project.ParentProjectId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", project.UserId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Project project = await db.Projects.FindAsync(id);
            db.Projects.Remove(project);
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
