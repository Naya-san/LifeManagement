using System;
using System.Collections.Generic;
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
    public class ProjectsController : ControllerExtensions
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var projects = db.Projects.Where(x => x.UserId == userId).Include(p => p.ParentProject);
            return View(await projects.ToListAsync());
        }

        public async Task<ActionResult> GetTasks(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return RedirectToAction("Index", "Cabinet");
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.ParentProjectId = new SelectList(db.Projects.Where(x => x.UserId == userId), "Id", "Path");
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create");
            }
            return RedirectToPrevious();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ParentProjectId,Name")] Project project)
        {
            var userId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                project.Id = Guid.NewGuid();
                project.UserId = userId;
                db.Projects.Add(project);
                await db.SaveChangesAsync();
               // return RedirectToPrevious();
                return Json(new { success = true });   
            }

            ViewBag.ParentProjectId = new SelectList(db.Projects.Where(x => x.Id != project.Id && x.UserId == userId), "Id", "Path", project.ParentProjectId);
            return PartialView("Create",project);
        }

        // GET: Projects/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return RedirectToAction("Index", "Cabinet");
            }
            var parentListTmp = db.Projects.Where(x => x.UserId.Equals(project.UserId)).ToList();
            var parentList = parentListTmp.Where(project1 => project1.CanHasLikeSon(project.Id));
            ViewBag.ParentProjectId = new SelectList(parentList, "Id", "Path", project.ParentProjectId);
            if (Request.IsAjaxRequest())
            {
                return PartialView("Edit", project);
            }
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
                return Json(new { success = true });   
            }
            var parentListTmp = db.Projects.Where(x => x.UserId.Equals(project.UserId)).ToList();
            var parentList = parentListTmp.Where(project1 => project1.CanHasLikeSon(project.Id));
            ViewBag.ParentProjectId = new SelectList(parentList, "Id", "Path", project.ParentProjectId);
            return PartialView("Edit", project);
        }

        // GET: Projects/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return RedirectToAction("Index", "Cabinet");
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Delete", project);
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var project = await db.Projects.FindAsync(id);
            db.Projects.RemoveRange(project.ChildProjects);
            db.Projects.Remove(project);
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
    }
}
