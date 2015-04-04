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
            return PartialView("Edit", usersetting);
        }

        // POST: /UserSetting/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,UserId,ComplexityLowFrom,ComplexityLowTo,ComplexityMediumTo,ComplexityHightTo")] UserSetting usersetting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usersetting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
           
            return PartialView("Edit", usersetting);
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
