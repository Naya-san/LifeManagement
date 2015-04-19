using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LifeManagement.Resources;

namespace LifeManagement.ViewModels
{
    public class TaskListSettingsViewModel
    {
        [Display(Name = "Date", ResourceType = typeof(ResourceScr))]
        public DateTime Date { set; get; }

        [Display(Name = "TimeToFill", ResourceType = typeof(ResourceScr))]
        public TimeSpan TimeToFill { set; get; }
    }
}