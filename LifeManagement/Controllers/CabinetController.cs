using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Extensions;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using LifeManagement.Resources;
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
                    ((
                    (x.StartDate.HasValue && x.StartDate.Value.Day <= FocusDate.Day && x.StartDate.Value.Month <= FocusDate.Month && x.StartDate.Value.Year <= FocusDate.Year) 
                    && ((x.EndDate.HasValue && x.EndDate.Value >= FocusDate) || !x.EndDate.HasValue)) 
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

        [Authorize]
        public ActionResult ShowStatisticResult()
        {
            return View();
        }

        public ActionResult StatisticsTasksPerDay()
        {
            var values = GetValesForGraf();
            var chart = new Chart(width: 900, height: 400, theme: ChartTheme.Yellow)
              .AddTitle(ResourceScr.StatisticsTasksPerDay)
              .AddSeries(
                     name: ResourceScr.QuantityTasks,
                     chartType: "Line",
                     xValue: values [0],
                     yValues: values[1])
              .AddLegend()
              .Write();

            return null;
        }

        public ActionResult StatisticsSuccess()
        {
            var values = GetValesForGrafFromArchives();
            var chart = new Chart(width: 900, height: 400, theme: ChartTheme.Yellow)
              .AddTitle(ResourceScr.StatisticsSuccess)
              .AddSeries(
                     name: ResourceScr.StatisticsSuccess,
                     chartType: "Line",
                     xValue: values[0],
                     yValues: values[1])
              .AddLegend()
              .Write();

            return null;
        }

        private string[][] GetValesForGraf()
        {
            var userId = User.Identity.GetUserId();
            const int N = 7;
            string[][] values = new [] { new string[7], new string[7]};
            var startDate = DateTime.UtcNow.Date.AddDays(-1*N);
            for (int i = 0; i < N; i++)
            {
                values[0][i] = startDate.ToString("d");
                values[1][i] = db.Records.OfType<Task>().Count(x => x.UserId == userId && x.CompletedOn.HasValue 
                    && x.CompletedOn.Value.Day == startDate.Day 
                    && x.CompletedOn.Value.Month == startDate.Month 
                    && x.CompletedOn.Value.Year == startDate.Year
                    ).ToString();
                startDate = startDate.AddDays(1);
            }
            return values;
        }
        private string[][] GetValesForGrafFromArchives()
        {
            var userId = User.Identity.GetUserId();
            const int N = 7;
            string[][] values = new[] { new string[7], new string[7] };
            var startDate = DateTime.UtcNow.Date.AddDays(-1 * N);
            for (int i = 0; i < N; i++)
            {
                values[0][i] = startDate.ToString("d");
                var list = db.ListsForDays.FirstOrDefault(x => x.UserId == userId && x.Date == startDate);
                values[1][i] = (list == null) ? "0" : list.CompleteLevel.ToString("##.");
                startDate = startDate.AddDays(1);
            }
            return values;
        }
    }
}