using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.ViewModels
{
    public class VersionsViewModel
    {
        public IList<ToDoList> ToDoLists { get; set; }

        public VersionsViewModel(IList<ToDoList>  toDoLists)
        {
            ToDoLists = toDoLists;
        }

        public VersionsViewModel()
        {
            ToDoLists = new List<ToDoList>();
        }
    }
}