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

        public virtual ICollection<Record> Records { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Archive> Archive { get; set; }

        protected ListForDay()
        {
            Id = Guid.NewGuid();
            Records = new List<Record>();
            Archive = new List<Archive>();
        }

        public ListForDay(DateTime date) : this()
        {
            Date = date;
        }
    }
}