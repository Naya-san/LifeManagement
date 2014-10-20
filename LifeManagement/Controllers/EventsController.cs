using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Enums;
using LifeManagement.Extensions;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using Microsoft.AspNet.Identity;
using Task = System.Threading.Tasks.Task;

namespace LifeManagement.Controllers
{
    [Authorize]
    [Localize]
    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Event>();
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
            return PartialView(records.ToList());
        }

        // GET: Events/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var @event = await db.Records.FindAsync(id) as Event;
            if (@event == null)
            {
                return HttpNotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name");
            ViewBag.Alerts = AlertPosition.None.ToSelectList();
            ViewBag.RepeatList = RepeatPosition.None.ToSelectList();

            HttpRequest request = System.Web.HttpContext.Current.Request;
            ViewBag.Date = request.GetUserLocalTimeFromUtc(DateTime.UtcNow).Date.ToString("yyyy-MM-dd");
            ViewBag.DateR = request.GetUserLocalTimeFromUtc(DateTime.UtcNow.AddYears(1)).Date.ToString("yyyy-MM-dd");
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create");
            }
            return RedirectToAction("Index", "Cabinet");
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Note,StartDate,EndDate,IsUrgent,StopRepeatDate")] Event @event)
        {
            var userId = User.Identity.GetUserId();
            HttpRequest request = System.Web.HttpContext.Current.Request;

            var time = Request["EndTime"].Split(':');
            var timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.EndDate = new DateTime(@event.EndDate.Value.Ticks).Add(timeSpan);
            time = Request["StartTime"].Split(':');
            timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.StartDate = new DateTime(@event.StartDate.Value.Ticks).Add(timeSpan);
            @event.EndDate = request.GetUtcFromUserLocalTime(@event.EndDate);
            @event.StartDate = request.GetUtcFromUserLocalTime(@event.StartDate);

            string[] listTag = new[] { "" };
            
            if (Request["Tags"] != "" && Request["Tags"] != null)
            {
                listTag = Request["Tags"].Split(',');
                foreach (string s in listTag)
                {
                    @event.Tags.Add(await db.Tags.FindAsync(new Guid(s)));
                }
            }
            int alertPosition = Int32.MinValue;
            if (Request["Alerts"] != null)
            {
                alertPosition = Convert.ToInt32(Request["Alerts"]);
            }

            var repeatPosition = (RepeatPosition)Enum.Parse(typeof(RepeatPosition), Request["RepeatList"], true);
            @event.RepeatPosition = repeatPosition;
            if (ModelState.IsValid)
            {
                @event.Id = Guid.NewGuid();
                @event.UserId = userId;
                if (alertPosition >= 0)
                {
                    var alert = new Alert { UserId = userId, RecordId = @event.Id, Id = Guid.NewGuid(), Date = @event.StartDate.Value.AddMinutes(-1 * alertPosition), Name = @event.Name };
                    @event.Alerts.Add(alert);
                    db.Alerts.Add(alert);
                }
                
                if (repeatPosition != RepeatPosition.None)
                {
                    var dateIter = new DateTime(@event.StartDate.Value.Ticks);
                    @event.GroupId = Guid.NewGuid();
                    while (dateIter.AddDays((int)repeatPosition) < @event.StopRepeatDate)
                    {
                        var newEvent = new Event
                        {
                            Id = Guid.NewGuid(),
                            GroupId = @event.GroupId,
                            Name = @event.Name,
                            Note = @event.Note,
                            UserId = @event.UserId,
                            IsUrgent = @event.IsUrgent,
                            RepeatPosition = @event.RepeatPosition,
                            StopRepeatDate = @event.StopRepeatDate
                        };
                        foreach (var tag in @event.Tags)
                        {
                            newEvent.Tags.Add(tag);
                        }
                        switch (repeatPosition)
                        {
                            case RepeatPosition.Ed:
                                dateIter = dateIter.AddDays(1);
                                newEvent.StartDate = new DateTime(dateIter.Ticks);
                                newEvent.EndDate = new DateTime(dateIter.Add(@event.EndDate.Value - @event.StartDate.Value).AddDays(1).Ticks);
                                break;
                            case RepeatPosition.Ew:
                                dateIter = dateIter.AddDays(7);
                                newEvent.StartDate = new DateTime(dateIter.Ticks);
                                newEvent.EndDate = new DateTime(dateIter.Add(@event.EndDate.Value - @event.StartDate.Value).AddDays(7).Ticks);
                                break;
                            case RepeatPosition.E2w:
                                dateIter = dateIter.AddDays(14);
                                newEvent.StartDate = new DateTime(dateIter.Ticks);
                                newEvent.EndDate = new DateTime(dateIter.Add(@event.EndDate.Value - @event.StartDate.Value).AddDays(14).Ticks);
                                break;
                            case RepeatPosition.Em:
                                dateIter = dateIter.AddMonths(1);
                                newEvent.StartDate = new DateTime(dateIter.Ticks);
                                newEvent.EndDate = new DateTime(dateIter.Add(@event.EndDate.Value - @event.StartDate.Value).AddMonths(1).Ticks);
                                break;
                            case RepeatPosition.Ey:
                                dateIter = dateIter.AddYears(1);
                                newEvent.StartDate = new DateTime(dateIter.Ticks);
                                newEvent.EndDate = new DateTime(dateIter.Add(@event.EndDate.Value - @event.StartDate.Value).AddYears(1).Ticks);
                                break;
                        }
                        foreach (var aler in @event.Alerts)
                        {
                            var newAlert = new Alert() { UserId = userId, Position = (AlertPosition) alertPosition, RecordId = newEvent.Id, Id = Guid.NewGuid(), Date = newEvent.StartDate.Value.AddMinutes(-1 * alertPosition), Name = @event.Name };
                            newEvent.Alerts.Add(newAlert);
                            db.Alerts.Add(newAlert);
                        }
                        db.Records.Add(newEvent);

                    } 

                }
                
                db.Records.Add(@event);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Cabinet");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var @event = await db.Records.FindAsync(id) as Event;
            if (@event == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();

            var selected = new string[@event.Tags.Count];
            int i = 0;
            foreach (var tag in @event.Tags)
            {
                selected[i] = tag.Id.ToString();
                i++;
            }
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name", selected);
            var alert = await db.Alerts.FirstOrDefaultAsync(x => x.UserId == userId && x.RecordId == @event.Id);
            ViewBag.Alerts = alert == null ? AlertPosition.None.ToSelectList() : alert.Position.ToSelectList();
            ViewBag.RepeatPosition = @event.RepeatPosition.ToSelectList();

            HttpRequest request = System.Web.HttpContext.Current.Request;
            @event.EndDate = request.GetUserLocalTimeFromUtc(@event.EndDate);
            @event.StartDate = request.GetUserLocalTimeFromUtc(@event.StartDate);

            if (Request.IsAjaxRequest())
            {
                return PartialView("Edit", @event);
            }
            return RedirectToAction("Index", "Cabinet");
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Note,StartDate,EndDate,IsUrgent")] Event @event)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            var time = Request["EndTime"].Split(':');
            var timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.EndDate = new DateTime(@event.EndDate.Value.Ticks).Add(timeSpan);
            time = Request["StartTime"].Split(':');
            timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.StartDate = new DateTime(@event.StartDate.Value.Ticks).Add(timeSpan);
            @event.EndDate = request.GetUtcFromUserLocalTime(@event.EndDate);
            @event.StartDate = request.GetUtcFromUserLocalTime(@event.StartDate);



            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Cabinet");
            }

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAll([Bind(Include = "Id,UserId,Name,Note,StartDate,EndDate,IsUrgent")] Event @event)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            var time = Request["EndTime"].Split(':');
            var timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.EndDate = new DateTime(@event.EndDate.Value.Ticks).Add(timeSpan);
            time = Request["StartTime"].Split(':');
            timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.StartDate = new DateTime(@event.StartDate.Value.Ticks).Add(timeSpan);
            @event.EndDate = request.GetUtcFromUserLocalTime(@event.EndDate);
            @event.StartDate = request.GetUtcFromUserLocalTime(@event.StartDate);



            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Cabinet");
            }

            return RedirectToAction("Edit",@event);
        }
        // GET: Events/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var task = await db.Records.FindAsync(id) as Event;
            if (task == null)
            {
                return HttpNotFound();
            }

            return View(task);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var task = await db.Records.FindAsync(id);
            db.Records.Remove(task);
            await db.SaveChangesAsync();
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
