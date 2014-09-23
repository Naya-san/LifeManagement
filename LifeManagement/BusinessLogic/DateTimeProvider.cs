using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.BusinessLogic
{
    public class DateTimeProvider
    {
        public virtual DateTime UtcNow {get { return DateTime.UtcNow; } }
    }
}