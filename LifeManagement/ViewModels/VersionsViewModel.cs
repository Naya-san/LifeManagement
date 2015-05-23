using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LifeManagement.Logic;
using LifeManagement.Models.DB;

namespace LifeManagement.ViewModels
{
    public class VersionsViewModel
    {
        public List<ToDoList> ToDoLists { get; set; }

        public VersionsViewModel(List<ToDoList>  toDoLists)
        {
            ToDoLists = toDoLists;
        }

        public VersionsViewModel()
        {
            ToDoLists = new List<ToDoList>();
        }

        public bool IsEmpty()
        {
            if (ToDoLists == null || !ToDoLists.Any())
            {
                return true;
            }
            if (Count() == 0)
            {
                return true;
            }
            return false;
        }

        public void Add(VersionsViewModel versions)
        {
            if (versions != null && !versions.IsEmpty())
            {
                ToDoLists.AddRange(versions.ToDoLists);   
            }
        }

        public int Count()
        {
            return ToDoLists.Count(todo => todo != null && todo.TasksTodo.Any());
        }
    }
}