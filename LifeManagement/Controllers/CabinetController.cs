using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Enums;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using Microsoft.AspNet.Identity;

namespace LifeManagement.Controllers
{
    [Localize]
    public class CabinetController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //
        // GET: /Cabinet/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult AllTasks()
        {
            return View();
        }
        [Authorize]
        public ActionResult AllEvents()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult()
        {
            ViewBag.Text = Request["TextForSearch"];
            return View();
        }

        [Authorize]
        public ActionResult Sidebar()
        {
            var userId = User.Identity.GetUserId();
            var projects = db.Projects.Where(p => p.UserId == userId);
            return View(projects.ToList());
        }

        [Authorize]
        public ActionResult TagsList()
        {
            var userId = User.Identity.GetUserId();
            var tags = db.Tags.Where(p => p.UserId == userId);
            return View(tags.ToList());
        }
	}
}