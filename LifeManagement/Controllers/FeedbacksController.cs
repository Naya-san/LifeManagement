using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Logic;
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

        // GET: Feedbacks/Create
        public ActionResult Create()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create");
            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
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
                EmailManager.SendMail(feedback, User.Identity.Name);
                return Json(new { success = true });
            }
            return PartialView("Create", feedback);
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
