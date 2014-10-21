using System;

namespace LifeManagement.ObsoleteSignalR
{
    public class RoutineTaskInfo
    {
        public String TaskId { get; set; }
        public String RoutineId { get; set; }
        public String Name { get; set; }
        public String Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String UserId { get; set; }
        public String Priority { get; set; }
        public String Complexity { get; set; }
        public double Readiness { get; set; }

        public string TimeString { get; set; }

        public double Duration { get
            {
                return EndDate.Subtract(StartDate).TotalMinutes;
            }
        }

    }
}