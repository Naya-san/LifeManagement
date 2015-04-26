using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LifeManagement.Enums;
using LifeManagement.Resources;

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

            [Display(Name = "ParallelismPercentage", ResourceType = typeof(ResourceScr))]
            public int ParallelismPercentage { get; set; }

            [Display(Name = "WorkingTime", ResourceType = typeof(ResourceScr))]    
            public TimeSpan WorkingTime { get; set; }

            [Display(Name = "TimeZone", ResourceType = typeof(ResourceScr))]
            public TimeSpan TimeZoneShift { get; set; }
            public virtual ApplicationUser User { get; set; }

            public UserSetting()
            {
                Id = Guid.NewGuid();
                SetDefault();
            }

            public UserSetting(string userId) : this()
            {
                UserId = userId;
            }

        public void SetDefault()
        {
            ComplexityLowFrom = new TimeSpan(0, 15, 0);
            ComplexityLowTo = new TimeSpan(1, 20, 0);
            ComplexityMediumTo = new TimeSpan(3, 30, 0);
            ComplexityHightTo = new TimeSpan(10, 30, 0);
            ParallelismPercentage = 30;
            WorkingTime = new TimeSpan(16, 0, 0);
            TimeZoneShift = new TimeSpan(0,2,0);
        }

        public TimeSpan[] GetRange(Complexity complexity)
        {
            var result = new TimeSpan[2];
            switch (complexity)
            {
                case Complexity.Low:
                    result[0] = ComplexityLowFrom;
                    result[1] = ComplexityLowTo;
                    break;
                case Complexity.Medium:
                    result[0] = ComplexityLowTo.Add(new TimeSpan(0, 1, 0));
                    result[1] = ComplexityMediumTo;
                    break;
                case Complexity.Hight:
                    result[0] = ComplexityMediumTo.Add(new TimeSpan(0,1,0));
                    result[1] = ComplexityHightTo;
                    break;
                default:
                    result[0] = new TimeSpan(0, 0, 0);
                    result[1] = new TimeSpan(0,15,0);
                    break;
            }
            return result;
        }


        public TimeSpan GetMaxComplexityRange(Complexity complexity)
        {
            switch (complexity)
            {
                case Complexity.Low:
                    return ComplexityLowTo;
                case Complexity.Medium:
                    return ComplexityMediumTo;
                case Complexity.Hight:
                    return ComplexityHightTo;
                default:
                    return new TimeSpan(0, 15, 0);
            }
        }
        public TimeSpan GetMinComplexityRange(Complexity complexity)
        {
            switch (complexity)
            {
                case Complexity.Low:
                    return ComplexityLowFrom;
                case Complexity.Medium:
                    return ComplexityLowTo;
                case Complexity.Hight:
                    return ComplexityMediumTo;
                default:
                    return new TimeSpan(0, 15, 0);
            }
        }

    }
}