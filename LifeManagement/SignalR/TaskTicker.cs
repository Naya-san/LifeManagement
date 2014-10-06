using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using LifeManagement.ObsoleteModels;
using Microsoft.AspNet.SignalR;
using System.Threading;
using Microsoft.AspNet.SignalR.Hubs;
using Timer = System.Threading.Timer;

namespace LifeManagement.SignalR
{
    public class TaskTicker
    {
        private static int frequency = 60;
        private readonly static Lazy<TaskTicker> _instance = new Lazy<TaskTicker>(() => new TaskTicker(GlobalHost.ConnectionManager.GetHubContext<TickerHub>().Clients));

        public static TaskTicker Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        private volatile LifeManagementContext db = new LifeManagementContext();

        private volatile bool _updating;
        private readonly object _updateLock = new object();
        private System.Threading.Timer _timer;

        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(frequency);
        public TaskTicker(IHubConnectionContext clients)
        {
            Clients = clients;
            _timer = new Timer(UpdateSpentTime, null, _updateInterval, _updateInterval);
        }

        private void UpdateSpentTime(object state)
        {
            lock (_updateLock)
            {
                if (!_updating)
                {
                    _updating = true;
                    db = new LifeManagementContext();
                    DateTime dateTime = DateTime.UtcNow;
                    List<DayLimit> dayList = db.DayLimits.Where(r => !r.IsDeleted && DateTime.Compare(r.StartDate, dateTime) <= 0 && DateTime.Compare(r.EndDate, dateTime) >= 0).ToList();
                   List<Routine> routines = new List<Routine>();
                    foreach (var dayLimit in dayList)
                    {
                        routines.Clear();
                        routines.AddRange(dayLimit.Routines.Where(r => !r.IsDeleted && r.Task.CompletedOn == null).OrderBy(r => r.StartDate));
                        foreach (var routine in routines)
                        {
                            if (routine.StartDate > dateTime)
                            {
                                break;
                            }
                            
                            routine.Task.SpentTime = routine.Task.SpentTime.Add(DateTime.Compare(dateTime.Add(_updateInterval), routine.EndDate) > 0 ? _updateInterval.Subtract(routine.EndDate.Subtract(dateTime)) : _updateInterval);

                            if (Math.Abs(routine.Task.Readiness - 100) < 0.01)
                            {
                                routine.Task.CompletedOn = routine.EndDate;
                                routine.Task.UpdatedOn = dateTime;
                            }

                        }
                    }
                    db.SaveChanges();
                    BroadcastSpentTime();
                    _updating = false;
                }
            }
        }

        private void BroadcastSpentTime()
        {
            Clients.All.updateSpentTime();
        }

        public IEnumerable<Routine> GetRoutines()
        {
            DateTime dateTime = DateTime.UtcNow;
            return db.Routines.Where(r => !r.IsDeleted && r.DayLimit != null && DateTime.Compare(r.DayLimit.StartDate, dateTime) <= 0 && DateTime.Compare(r.DayLimit.EndDate, dateTime) >= 0).ToList();
        }
    }
}