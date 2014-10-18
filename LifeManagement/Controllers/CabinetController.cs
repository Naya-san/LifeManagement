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
            //var userId = User.Identity.GetUserId();
            //var userGuid = Guid.Parse(userId);

            //var request = System.Web.HttpContext.Current.Request;
            //var records = db.Records.Where(x => x.UserId == userId).OfType<Task>().OrderBy(x => x.StartDate).ToList();

            //foreach (var record in records)
            //{
            //    if (record.StartDate.HasValue)
            //    {
            //        record.StartDate = UserTimeConverter.GetUserLocalTimeFromUtc(userGuid, request, record.StartDate.Value);
            //    }

            //    if (record.EndDate.HasValue)
            //    {
            //        record.EndDate = UserTimeConverter.GetUserLocalTimeFromUtc(userGuid, request, record.EndDate.Value);
            //    }
            //}
            //return View(records);
            if (Session["CabinetState"] == null)
            {
                Session["CabinetState"] = CabinetState.Focus;
            }
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