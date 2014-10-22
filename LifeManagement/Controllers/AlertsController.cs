using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Extensions;
using LifeManagement.Models;
using LifeManagement.ViewModels;
using Microsoft.AspNet.Identity;

namespace LifeManagement.Controllers
{
    [System.Web.Mvc.Authorize]
    [Localize]
    public class AlertsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Alerts
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var alerts = db.Alerts.Where(x => x.UserId == userId && x.Date <= DateTime.UtcNow).ToList()
                .Select(x => new AlertViewModel{ Id = x.Id, Name = x.Name, Date = request.GetUserLocalTimeFromUtc(x.Date).ToString()});

            return View(alerts.ToList());
        }

        // GET: Alerts/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
           //return View(alert);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var alert = await db.Alerts.FindAsync(id);
            if (alert == null)
            {
                return HttpNotFound();
            }
            db.Alerts.Remove(alert);
            await db.SaveChangesAsync();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        // POST: Alerts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var alert = await db.Alerts.FindAsync(id);
            db.Alerts.Remove(alert);
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
