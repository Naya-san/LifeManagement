using LifeManagement.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.ViewModels
{
    public class ToDoList
    {
        private List<Task> tasksTodo;

        public List<Task> TasksTodo { get { return tasksTodo; } }

        public TimeSpan TimeEstimate { private set; get; }

        public UserSetting UserSetting { private set; get; }

        public void AddTask(Task value)
        {
            tasksTodo.Add(value);
            TimeEstimate += value.CalculateTimeLeft(UserSetting);
        }

        public void AddTasksRange(IList<Task> value)
        {
            tasksTodo.AddRange(value);
            foreach (var task in value)
            {
                TimeEstimate += task.CalculateTimeLeft(UserSetting);
            }
        }
        public ToDoList(UserSetting userSetting)
        {
            tasksTodo = new List<Task>();
            TimeEstimate = new TimeSpan(0);
            UserSetting = userSetting;
        }

    }
}