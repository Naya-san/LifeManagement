using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.ViewModels
{
    public class TaskListSettingsViewModel
    {
        public DateTime Date { set; get; }
        public TimeSpan TimeToFill { set; get; }
    }
}