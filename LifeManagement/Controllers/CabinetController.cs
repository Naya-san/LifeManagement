using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Extensions;
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

        [Authorize]
        public ActionResult SearchResult()
        {
            if (Request["TextForSearch"] != null)
            {
                Session["Text"] = Request["TextForSearch"];
            }
            if (Session["Text"] == null || Session["Text"] == "")
            {
                return RedirectToAction("Index", "Cabinet");
            }
            return View();
        }

        [Authorize]
        public ActionResult TimetableOnDate()
        {
            if (Request["FocusDate"] != null)
            {
                Session["FocusDate"] = Request["FocusDate"];
            }
            if (Session["FocusDate"] == null || Session["FocusDate"] == "")
            {
                return RedirectToAction("Index", "Cabinet");
            }
            var FocusDate = DateTime.Parse(Session["FocusDate"].ToString());
            var userId = User.Identity.GetUserId();
            var records =
                db.Records.Where(x => x.UserId == userId && 
                    (((x.StartDate.HasValue && x.StartDate.Value <= FocusDate) && ((x.EndDate.HasValue && x.EndDate.Value >= FocusDate) || !x.EndDate.HasValue)) 
                    || 
                    (!x.StartDate.HasValue && x.EndDate.HasValue && x.EndDate.Value >= FocusDate))).ToList();
            var recordsTmp = records.OfType<Task>().Where(x  => x.CompletedOn != null).ToList();
            foreach (var rec in recordsTmp)
            {
                records.Remove(rec);
            }
            var request = System.Web.HttpContext.Current.Request;
            foreach (var record in records)
            {
                if (record.StartDate.HasValue)
                {
                    record.StartDate = request.GetUserLocalTimeFromUtc(record.StartDate.Value);
                }

                if (record.EndDate.HasValue)
                {
                    record.EndDate = request.GetUserLocalTimeFromUtc(record.EndDate.Value);
                }
            }
            return View(records);
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