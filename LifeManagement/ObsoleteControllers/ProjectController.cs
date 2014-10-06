using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.ObsoleteModels;
using Microsoft.AspNet.Identity;

namespace LifeManagement.ObsoleteControllers
{
    [Localize]
    public class ProjectController : Controller
    {
        private LifeManagementContext db = new LifeManagementContext();

        // GET: /Project/Create
        public ActionResult Create(Guid? parentId)
        {
            var userId = new Guid(User.Identity.GetUserId());
            if (parentId == null)
            {
                ViewBag.ParentProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(userId)), "Id", "Path", String.Empty);
                return View();
            }

            Project project = db.Projects.Find(parentId);
            if (project == null || project.IsDeleted || !project.UserId.Equals(userId))
            {
                return HttpNotFound();
            }
            ViewBag.ParentProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(userId)), "Id", "Path", project.Id);
            return View();
        }

        // POST: /Project/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id, Name,ParentProjectId")] Project project)
        {
            project.IsDeleted = false;
            project.UpdatedOn = DateTime.UtcNow;

            project.UserId = new Guid(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                project.Id = Guid.NewGuid();
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index", "Cabinet");
            }

            ViewBag.ParentProjectId = new SelectList(db.Projects, "Id", "Path", project.ParentProjectId);
            return View(project);
        }
        
        // GET: /Project/Edit/5
        [Authorize]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            var userId = new Guid(User.Identity.GetUserId());
            if (project == null || project.IsDeleted || !project.UserId.Equals(userId))
            {
                return HttpNotFound();
            }
            Guid userGuid = new Guid(User.Identity.GetUserId());
            SelectList list = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(userGuid)), "Id",
                "Path", project.ParentProjectId);
            if (project.ParentProjectId.Equals(Guid.Empty) || project.ParentProjectId == null)
            {
                ViewBag.ParentProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(userGuid) && !p.Id.Equals(project.Id)), "Id", "Path", String.Empty);
            }
            else
            {
                ViewBag.ParentProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(userGuid) && !p.Id.Equals(project.Id)), "Id", "Path", project.ParentProjectId);
            }
            return View(project);
        }

        // POST: /Project/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,UpdatedOn,IsDeleted,Name,UserId,ParentProjectId")] Project project)
        {
            project.UpdatedOn = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Cabinet");
            }
            ViewBag.ParentProjectId = new SelectList(db.Projects.Where(p => !p.IsDeleted && p.UserId.Equals(project.UserId)), "Id", "Path", project.ParentProjectId);
            return View(project);
        }

        // GET: /Project/Delete/5
         [Authorize]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            var userId = new Guid(User.Identity.GetUserId());
            if (project == null || project.IsDeleted || !project.UserId.Equals(userId))
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: /Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Project project = db.Projects.Find(id);
            project.IsDeleted = true;
            project.UpdatedOn = DateTime.UtcNow;
            foreach (var task in project.Tasks)
            {
                task.IsDeleted = true;
                task.UpdatedOn = DateTime.UtcNow;
            }
            db.SaveChanges();
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
