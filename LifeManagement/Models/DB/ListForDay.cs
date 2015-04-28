using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.Models.DB
{
    public class ListForDay : IEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }

        public DateTime Date { get; set; }

        public double CompleteLevel { get; set; }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Archive> Archive { get; set; }

        protected ListForDay()
        {
            Id = Guid.NewGuid();
            Archive = new List<Archive>();
            Events = new List<Event>();
        }

        public ListForDay(DateTime date) : this()
        {
            Date = date;
        }

        public TimeSpan EventTime(UserSetting settings)
        {
            double minutes = 0;
            foreach (var @event in Events)
            {
                minutes += @event.CalculateTimeLeftInDay(Date, settings);
            }
            return TimeSpan.FromMinutes(minutes);
        }
        public TimeSpan TaskTime(UserSetting settings)
        {
            double minutes = 0;
            foreach (var archive in Archive)
            {
                minutes += (archive.LevelOnEnd - archive.LevelOnStart) * settings.GetMinComplexityRange(archive.Task.Complexity).TotalMinutes / 100.0;
            }
            return TimeSpan.FromMinutes(minutes);
        }
    }
}