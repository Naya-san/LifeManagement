using System;
using System.Collections.Generic;
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
using LifeManagement.Resources;
using Microsoft.AspNet.Identity;

namespace LifeManagement.Controllers
{
    [Authorize]
    [Localize]
    public class EventsController : ControllerExtensions
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult FilterEvents(RecordFilter recordFilter)
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Event>().ToList();
            records = FilterRecords(records, recordFilter);
            ConvertEventsToUserLocalTime(request, records);
            return PartialView("Index", records);
        }

        public ActionResult GetEventsByTag(Guid tagId)
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Event>().Include(x => x.Tags).ToList().Where(x => x.Tags.Any(y => y.Id == tagId) && x.EndDate != null && x.EndDate.Value >= DateTime.UtcNow);
            ConvertEventsToUserLocalTime(request, records);
            return PartialView("Index", records);
        }

        public ActionResult GetEventsByText(string text)
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Event>().ToList().Where(x=>x.Name.ToLower().Contains(text.ToLower()));
            ConvertEventsToUserLocalTime(request, records);
            return PartialView("Index", records);
        }

        // GET: Events
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var request = System.Web.HttpContext.Current.Request;
            var records = db.Records.Where(x => x.UserId == userId).OfType<Event>().ToList();
            ConvertEventsToUserLocalTime(request, records);
            return PartialView(records);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name");
            ViewBag.Alerts = AlertPosition.None.ToSelectList();
            ViewBag.RepeatPosition = RepeatPosition.None.ToSelectList();

            HttpRequest request = System.Web.HttpContext.Current.Request;
            var now = request.GetUserLocalTimeFromUtc(DateTime.UtcNow);
            ViewBag.DateStart = now.Date;
            ViewBag.TimeStart = now.ToTimeFormat();
            ViewBag.DateEnd = now.AddHours(1).Date;
            ViewBag.TimeEnd = now.AddHours(1).ToTimeFormat();
            ViewBag.DateR = request.GetUserLocalTimeFromUtc(DateTime.UtcNow.AddMonths(2)).Date;
            ViewBag.BlockState = "closeBlock";
            ViewBag.RepeatState = "none";
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create");
            }
            return RedirectToPrevious();
        }

        private void AddReperts(Event @event, int alertPosition, HttpRequest request)
        {
            db.Records.Attach(@event);
            db.Entry(@event).Collection(x => x.Tags).Load();
            db.Entry(@event).Collection(x => x.Alerts).Load();

            if (@event.RepeatPosition != RepeatPosition.None)
            {
                var dateIter = new DateTime(@event.StartDate.Value.Ticks);
                if (@event.GroupId.Equals(Guid.Empty))
                {
                    @event.GroupId = Guid.NewGuid();
                }
                
                while (dateIter.AddDays((int)@event.RepeatPosition) < @event.StopRepeatDate)
                {
                    var newEvent = new Event
                    {
                        Id = Guid.NewGuid(),
                        GroupId = @event.GroupId,
                        Name = @event.Name,
                        Note = @event.Note,
                        UserId = @event.UserId,
                        IsImportant = @event.IsImportant,
                        RepeatPosition = @event.RepeatPosition,
                        StopRepeatDate = @event.StopRepeatDate,
                        OnBackground = @event.OnBackground
                    };
                    foreach (var tag in @event.Tags)
                    {
                        newEvent.Tags.Add(tag);
                    }
                    switch (@event.RepeatPosition)
                    {
                        case RepeatPosition.Ed:
                            dateIter = dateIter.AddDays(1);
                            newEvent.StartDate = new DateTime(dateIter.Ticks);
                            newEvent.EndDate = new DateTime(newEvent.StartDate.Value.Add(@event.EndDate.Value - @event.StartDate.Value).Ticks);
                            break;
                        case RepeatPosition.Ew:
                            dateIter = dateIter.AddDays(7);
                            newEvent.StartDate = new DateTime(dateIter.Ticks);
                            newEvent.EndDate = new DateTime(newEvent.StartDate.Value.Add(@event.EndDate.Value - @event.StartDate.Value).Ticks);
                            break;
                        case RepeatPosition.E2w:
                            dateIter = dateIter.AddDays(14);
                            newEvent.StartDate = new DateTime(dateIter.Ticks);
                            newEvent.EndDate = new DateTime(newEvent.StartDate.Value.Add(@event.EndDate.Value - @event.StartDate.Value).Ticks);
                            break;
                        case RepeatPosition.Em:
                            dateIter = dateIter.AddMonths(1);
                            newEvent.StartDate = new DateTime(dateIter.Ticks);
                            newEvent.EndDate = new DateTime(newEvent.StartDate.Value.Add(@event.EndDate.Value - @event.StartDate.Value).Ticks);
                            break;
                        case RepeatPosition.Ey:
                            dateIter = dateIter.AddYears(1);
                            newEvent.StartDate = new DateTime(dateIter.Ticks);
                            newEvent.EndDate = new DateTime(newEvent.StartDate.Value.Add(@event.EndDate.Value - @event.StartDate.Value).Ticks);
                            break;
                    }
                    foreach (var aler in @event.Alerts)
                    {
                        var newAlert = new Alert() { UserId = @event.UserId, Position = (AlertPosition)alertPosition, RecordId = newEvent.Id, Id = Guid.NewGuid(), Date = newEvent.StartDate.Value.AddMinutes(-1 * alertPosition), Name = String.Concat(@event.Name, Resources.ResourceScr.at, request.GetUserLocalTimeFromUtc(newEvent.StartDate).Value.ToString("g")) };
                        newEvent.Alerts.Add(newAlert);
                        db.Alerts.Add(newAlert);
                    }
                    db.Records.Add(newEvent);

                }

            }
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Note,StartDate,EndDate,IsImportant,StopRepeatDate,OnBackground")] Event @event)
        {
            var userId = User.Identity.GetUserId();
            HttpRequest request = System.Web.HttpContext.Current.Request;

            var time = Request["EndTime"].Split(':');
            var timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.EndDate = new DateTime(@event.EndDate.Value.Ticks).Add(timeSpan);
            time = Request["StartTime"].Split(':');
            timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.StartDate = new DateTime(@event.StartDate.Value.Ticks).Add(timeSpan);
            string startString = @event.StartDate.Value.ToString("g");
            var now = request.GetUserLocalTimeFromUtc(DateTime.UtcNow);
            ViewBag.DateStart = @event.StartDate.HasValue ? @event.StartDate.Value.Date : now.Date;
            ViewBag.TimeStart = @event.StartDate.HasValue ? @event.StartDate.Value.ToTimeFormat() : now.ToTimeFormat();
            ViewBag.DateEnd = @event.EndDate.HasValue ? @event.EndDate.Value.Date : now.AddHours(1).Date;
            ViewBag.TimeEnd = @event.EndDate.HasValue ? @event.EndDate.Value.ToTimeFormat() : now.AddHours(1).ToTimeFormat();

            @event.StartDate = request.GetUtcFromUserLocalTime(@event.StartDate);
            @event.EndDate = request.GetUtcFromUserLocalTime(@event.EndDate);
            @event.StopRepeatDate = request.GetUtcFromUserLocalTime(@event.StopRepeatDate);

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

            var repeatPosition = (RepeatPosition)Enum.Parse(typeof(RepeatPosition), Request["RepeatPosition"], true);
            @event.RepeatPosition = repeatPosition;
            if (ModelState.IsValid && @event.IsTimeValid(ModelState, AlertPosition.None.Parse(alertPosition)))
            {
                @event.Id = Guid.NewGuid();
                @event.UserId = userId;
                if (alertPosition >= 0)
                {
                    var alert = new Alert { UserId = userId, RecordId = @event.Id, Id = Guid.NewGuid(), Date = @event.StartDate.Value.AddMinutes(-1 * alertPosition), Name = String.Concat(@event.Name, Resources.ResourceScr.at, startString) };
                    @event.Alerts.Add(alert);
                    db.Alerts.Add(alert);
                }
                AddReperts(@event, alertPosition, request);
              
                db.Records.Add(@event);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            
            ViewBag.DateR = (@event.StopRepeatDate.HasValue) ? request.GetUserLocalTimeFromUtc(@event.StopRepeatDate).Value.Date : request.GetUserLocalTimeFromUtc(now.AddMonths(2));
      
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == userId), "Id", "Name", listTag);

            ViewBag.Alerts = AlertPosition.None.Parse(alertPosition).ToSelectList();
            ViewBag.RepeatPosition = @event.RepeatPosition.ToSelectList();
            ViewBag.BlockState = "openBlock";
            ViewBag.RepeatState = @event.RepeatPosition == RepeatPosition.None ? "none" : "block";
            return PartialView("Create", @event);
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
            return RedirectToPrevious();
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,GroupId,Name,Note,StartDate,EndDate,IsImportant,RepeatPosition,StopRepeatDate,OnBackground")] Event @event)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            var time = Request["EndTime"].Split(':');
            var timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.EndDate = new DateTime(@event.EndDate.Value.Ticks).Add(timeSpan);
            time = Request["StartTime"].Split(':');
            timeSpan = new TimeSpan(0, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            @event.StartDate = new DateTime(@event.StartDate.Value.Ticks).Add(timeSpan);
            string startString = @event.StartDate.Value.ToString("g");
            @event.EndDate = request.GetUtcFromUserLocalTime(@event.EndDate);
            @event.StartDate = request.GetUtcFromUserLocalTime(@event.StartDate);
            

            db.Records.Attach(@event);
            db.Entry(@event).Collection(x => x.Tags).Load();
            db.Entry(@event).Collection(x => x.Alerts).Load();

            bool flagNeedRewrite = true;
            if (Request["Tags"] != "" && Request["Tags"] != null)
            {
                List<Tag> newTags = new List<Tag>();
                var tags = Request["Tags"].Split(',');
                foreach (string s in tags)
                {
                    newTags.Add(await db.Tags.FindAsync(new Guid(s)));
                }
                List<Tag> newTagsTmp = @event.Tags.ToList();
                foreach (var tag in newTagsTmp)
                {
                    if (!newTags.Contains(tag))
                    {
                        @event.Tags.Remove(tag);
                    }
                }
                foreach (var tag in newTags)
                {
                    if (!@event.Tags.Contains(tag))
                    {
                        @event.Tags.Add(tag);
                    }
                }
            }
            if (ModelState.IsValid && @event.IsTimeValid(ModelState, AlertPosition.None))
            {
                int alertPosition = Int32.MinValue;
                if (Request["Alerts"] != null)
                {
                    alertPosition = Convert.ToInt32(Request["Alerts"]);
                }
                Alert alert = (@event.Alerts != null && @event.Alerts.Any()) ? @event.Alerts.ToArray()[0] : null;
                if (alertPosition >= 0 && alert != null)
                {
                    if (alertPosition != (int)alert.Position || (@event.StartDate.Value - alert.Date).TotalMinutes != alertPosition)
                    {
                        alert.Name = String.Concat(@event.Name, ResourceScr.at, startString);
                        alert.Date = @event.StartDate.Value.AddMinutes(-1 * alertPosition);
                        alert.Position = (AlertPosition)alertPosition;
                    }
                }
                else
                {
                    if (alertPosition >= 0 && alert == null)
                    {
                        alert = new Alert { UserId = @event.UserId, Position = (AlertPosition)alertPosition, RecordId = @event.Id, Id = Guid.NewGuid(), 
                            Date = @event.StartDate.Value.AddMinutes(-1 * alertPosition), Name = String.Concat(@event.Name, ResourceScr.at, startString) };
                        db.Alerts.Add(alert);
                    }
                    else
                    {
                        if (alertPosition < 0 && alert != null)
                        {
                            db.Alerts.Remove(alert);
                        }
                    }
                }
                if (@event.RepeatPosition == RepeatPosition.None && !@event.GroupId.Equals(Guid.Empty))
                {
                    var repeatsList =
                        db.Records.Where(x => x.UserId == @event.UserId)
                            .OfType<Event>()
                            .Where(x => x.GroupId == @event.GroupId && x.StartDate > @event.StartDate)
                            .ToList();
                    
                    foreach (var _event in repeatsList)
                    {
                        db.Records.Attach(_event);
                        db.Records.Remove(_event);
                    }
                    @event.GroupId = Guid.Empty;
                    flagNeedRewrite = false;
                }
                else
                {
                    if (@event.RepeatPosition != RepeatPosition.None && @event.GroupId.Equals(Guid.Empty))
                    {
                        AddReperts(@event, alertPosition, request);
                        flagNeedRewrite = false;
                    }
                    else
                    {
                        var repeatsList =
                            db.Records.Where(x => x.UserId == @event.UserId)
                                .OfType<Event>()
                                .Where(x => x.GroupId == @event.GroupId)
                                .ToList();
                        if (repeatsList[0].RepeatPosition != @event.RepeatPosition ||
                            repeatsList[0].StopRepeatDate != @event.StopRepeatDate)
                        {
                            foreach (var _event in repeatsList.Where(x => x.StartDate > @event.StartDate))
                            {
                                db.Records.Attach(_event);
                                db.Records.Remove(_event);
                            }
                            @event.GroupId = Guid.NewGuid();
                            AddReperts(@event, alertPosition, request);
                            flagNeedRewrite = false;
                        }
                    }
                }
                if (Request["ForAll"] != null)
                {
                    bool forAll = !Request["ForAll"].Equals("false");
                    if (forAll && flagNeedRewrite && !@event.GroupId.Equals(Guid.Empty))
                    {
                        var repeatsList = db.Records.Where(x => x.UserId == @event.UserId).OfType<Event>().Where(x => x.GroupId == @event.GroupId && x.StartDate > @event.StartDate).ToList();
                        if (repeatsList != null && repeatsList.Any())
                        {
                            foreach (var _event in repeatsList)
                            {
                                db.Records.Attach(_event);
                                db.Records.Remove(_event);
                            }
                            AddReperts(@event, alertPosition, request);
                        }
                    }
                }
               
                db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            var selected = new string[@event.Tags.Count];
            int i = 0;
            foreach (var tag in @event.Tags)
            {
                selected[i] = tag.Id.ToString();
                i++;
            }
            ViewBag.Tags = new MultiSelectList(db.Tags.Where(x => x.UserId == @event.UserId), "Id", "Name", selected);
            ViewBag.Alerts = Request["Alerts"] == null ? AlertPosition.None.ToSelectList() : AlertPosition.None.Parse(Convert.ToInt32(Request["Alerts"])).ToSelectList();
            ViewBag.RepeatPosition = @event.RepeatPosition.ToSelectList();

            @event.EndDate = request.GetUserLocalTimeFromUtc(@event.EndDate);
            @event.StartDate = request.GetUserLocalTimeFromUtc(@event.StartDate);
            return PartialView("Edit",@event);
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
            if (Request.IsAjaxRequest())
            {
                return PartialView("Delete", task);
            }
            return RedirectToPrevious();
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var task = await db.Records.FindAsync(id);
            db.Records.Remove(task);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected void ConvertEventsToUserLocalTime(HttpRequest request, IEnumerable<Event> records)
        {
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
        }

        protected List<Event> FilterRecords(List<Event> records, RecordFilter recordFilter)
        {
            switch (recordFilter)
            {
                case RecordFilter.Today:
                    {
                        var dueDate = DateTime.UtcNow;
                        return
                            records.Where(
                                x =>
                                    (x.StartDate != null && x.StartDate.Value <= dueDate) &&
                                    (x.EndDate != null && x.EndDate.Value >= dueDate)).ToList();
                    }
                case RecordFilter.Tomorrow:
                    {
                        var dueDate = DateTime.UtcNow.Date.AddDays(1);
                        return
                            records.Where(
                                x =>
                                    (x.StartDate != null && x.StartDate.Value.Date <= dueDate) &&
                                    (x.EndDate != null && x.EndDate.Value.Date >= dueDate)).ToList();
                    }
                case RecordFilter.Future:
                    {
                        var dueDate = DateTime.UtcNow.Date;
                        return
                            records.Where(
                                x =>
                                    (x.StartDate != null && x.StartDate.Value.Date > dueDate)).ToList();
                    }
                default:
                    return records;
            }
        } 
    }
}
