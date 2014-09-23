using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LifeManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Task = System.Threading.Tasks.Task;

namespace LifeManagement.SignalR
{
   // [HubName("lifeHub")]
    public class TickerHub : Hub
    {
        private readonly static ConnectionMapping<string> Connections = new ConnectionMapping<string>();

        private readonly TaskTicker _taskTicker;

        public TickerHub() : this(TaskTicker.Instance) { }

        private TickerHub(TaskTicker instance)
        {
            _taskTicker = instance;
        }

         [HubMethodName("getRoutinesInfo")]
         public void GetRoutinesInfo()
         {
             var routines = _taskTicker.GetRoutines().ToList();
             string userId = Context.User.Identity.GetUserId();
             var userGuid = new Guid(Context.User.Identity.GetUserId());
             foreach (var connectionId in Connections.GetConnections(userId))
             {
                 List<Routine> routinesTmp = routines.Where(r => r.UserId.Equals(userGuid)).OrderBy(x => x.StartDate).ToList();
                 List<RoutineTaskInfo> infos = routinesTmp.Select(routine => new RoutineTaskInfo()
                                                                             {
                                                                                 TaskId = routine.TaskId.ToString(), RoutineId = routine.Id.ToString(), Name = routine.Task.Name,
                                                                                 Type = routine.Type.ToString(), StartDate = routine.StartDate, 
                                                                                 EndDate = routine.EndDate, UserId = routine.UserId.ToString(), TimeString = routine.TimeString,
                                                                                 Priority = routine.Task.Priority.ToString(),
                                                                                 Complexity = routine.Task.Complexity.ToString(), Readiness = routine.Task.Readiness
                                                                             }).ToList();
                 Clients.Client(connectionId).setUsersRoutinesInfo(infos);
             }
         } 

        public override Task OnConnected()
        {
            string userId = Context.User.Identity.GetUserId();

            Connections.Add(userId, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            string userId = Context.User.Identity.GetUserId();

            Connections.Remove(userId, Context.ConnectionId);

            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            string userId = Context.User.Identity.GetUserId();

            if (!Connections.GetConnections(userId).Contains(Context.ConnectionId))
            {
                Connections.Add(userId, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
    }
}