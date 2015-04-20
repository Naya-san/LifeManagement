using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.Models.DB
{
    public class Archive: IEntity
    {
        public Guid Id{get; set;}

        public Guid TaskId { get; set; }

        public int LevelOnStart { get; set; }

        public int LevelOnEnd { get; set; }

        public virtual Task Task{get;set;}

        
    }
}