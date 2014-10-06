using System;

namespace LifeManagement.ObsoleteBusinessLogic
{
    public class DateTimeProvider
    {
        public virtual DateTime UtcNow {get { return DateTime.UtcNow; } }
    }
}