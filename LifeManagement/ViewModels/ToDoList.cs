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

        public double Score = 0;

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

        public bool ConteinsTask(Task task)
        {
            return TasksTodo.Contains(task);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ToDoList))
            {
                return false;
            }
            var todoList = obj as ToDoList;
            if (todoList.TimeEstimate.Ticks != TimeEstimate.Ticks || tasksTodo.Count != todoList.tasksTodo.Count)
            {
                return false;
            }
            return !TasksTodo.Where((t, i) => !todoList.ConteinsTask(t) || !tasksTodo.Contains(todoList.TasksTodo[i])).Any();
        }

        public override int GetHashCode()
        {
            return TimeEstimate.Ticks.GetHashCode();
        }

        public void SortTasks()
        {
            tasksTodo = tasksTodo.OrderByDescending(t => t.IsImportant).ThenByDescending(t => t.Complexity).ToList();
        }
    }
}