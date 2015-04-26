using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Models.DB;
using LifeManagement.Models;
using Microsoft.AspNet.Identity;

namespace LifeManagement.Controllers
{

    [Authorize]
    [Localize]
    public class UserSettingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /UserSetting/Edit/5
        public async Task<ActionResult> Edit()
        {
            var userId = User.Identity.GetUserId();
            UserSetting usersetting = await db.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId);
            if (usersetting == null)
            {
                usersetting = new UserSetting();
                usersetting.UserId = userId;
                db.UserSettings.Add(usersetting);
                await db.SaveChangesAsync();
            }
            ViewBag.TimeZoneShift = CreateTimeZoneList(usersetting.TimeZoneShift);
            return PartialView("Edit", usersetting);
        }


        // POST: /UserSetting/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,ComplexityLowFrom,ComplexityLowTo,ComplexityMediumTo,ComplexityHightTo,ParallelismPercentage,WorkingTime,TimeZoneShift")] UserSetting usersetting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usersetting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewBag.TimeZoneShift = CreateTimeZoneList(usersetting.TimeZoneShift);
            return PartialView("Edit", usersetting);
        }

        private SelectList CreateTimeZoneList(TimeSpan timeShift)
        {
            List<KeyValuePair<TimeSpan, string>> timeList = new List<KeyValuePair<TimeSpan, string>>()
            {
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-12, 0, 0), "UTC-12"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-11, 0, 0), "UTC-11"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-10, 0, 0), "UTC-10"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-9, 0, 0), "UTC-9"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-8, 0, 0), "UTC-8"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-7, 0, 0), "UTC-7"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-6, 0, 0), "UTC-6"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-5, 0, 0), "UTC-5"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-4, 30, 0), "UTC-4:30"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-4, 0, 0), "UTC-4"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-3, 30, 0), "UTC-3:30"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-2, 0, 0), "UTC-2"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(-1, 0, 0), "UTC-1"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(0, 0, 0), "UTC+0"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(1, 0, 0), "UTC+1"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(2, 0, 0), "UTC+2"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(3, 0, 0), "UTC+3"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(3, 30, 0), "UTC+3:30"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(4, 0, 0), "UTC+4"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(4, 30, 0), "UTC+4:30"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(5, 0, 0), "UTC+5"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(5, 30, 0), "UTC+5:30"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(5, 45, 0), "UTC+5:45"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(6, 0, 0), "UTC+6"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(6, 30, 0), "UTC+6:30"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(7, 0, 0), "UTC+7"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(8, 0, 0), "UTC+8"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(8, 45, 0), "UTC+8:45"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(9, 0, 0), "UTC+9"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(9, 30, 0), "UTC+9:30"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(10, 0, 0), "UTC+10"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(10, 30, 0), "UTC+10:30"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(11, 0, 0), "UTC+11"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(11, 30, 0), "UTC+11:30"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(12, 0, 0), "UTC+12"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(12, 45, 0), "UTC+12:45"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(13, 0, 0), "UTC+13"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(13, 45, 0), "UTC+13:45"),
                new KeyValuePair<TimeSpan, string>(new TimeSpan(14, 0, 0), "UTC+14")
            };
            return new SelectList(timeList, "Key", "Value", timeShift);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<ActionResult> ToDefault([Bind(Include = "Id,UserId,ComplexityLowFrom,ComplexityLowTo,ComplexityMediumTo,ComplexityHightTo,ParallelismPercentage,WorkingTime")] UserSetting settings)
        {
            if (settings == null)
            {
                return HttpNotFound();
            }
            settings.SetDefault();
            if (ModelState.IsValid)
            {
                db.Entry(settings).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            return Json(new { success = true });
        }
    }
}
