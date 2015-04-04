using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.Models.DB
{
    public class UserSetting : IEntity
    {
            public Guid Id { get; set; }
            public string UserId { get; set; }
            public TimeSpan ComplexityLowFrom { get; set; }
            public TimeSpan ComplexityLowTo { get; set; }
            public TimeSpan ComplexityMediumTo { get; set; }
            public TimeSpan ComplexityHightTo { get; set; }

            public virtual ApplicationUser User { get; set; }

            public UserSetting()
            {
                Id = Guid.NewGuid();
                ComplexityLowFrom = new TimeSpan(0, 5, 0);
                ComplexityLowTo = new TimeSpan(1, 20, 0);
                ComplexityMediumTo = new TimeSpan(3, 30, 0);
                ComplexityHightTo = new TimeSpan(10, 30, 0);
            }
    }
}