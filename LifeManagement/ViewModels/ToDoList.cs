using LifeManagement.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.ViewModels
{
    public class ToDoList
    {
        public List<Task> TasksTodo { set; get;}

        public TimeSpan TimeEstimate { set; get; }
    }
}