#region License
// Copyright (c) 2014 Life Management Team
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LifeManagement.Enums;
using LifeManagement.Resources;
using Newtonsoft.Json;

namespace LifeManagement.Models
{
    public class Task : ISynchronizableEntity
    {
        public virtual Guid Id { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(ResourceScr))]
        [StringLength(25, ErrorMessageResourceName = "ErrorStrLen", ErrorMessageResourceType = typeof(ResourceScr))]
        [RegularExpression(@"[A-Za-zА-Яа-яА-Яа-я0-9,:._()\-\s\""]+",
        ErrorMessageResourceName = "ErrorRegulExpr", ErrorMessageResourceType = typeof(ResourceScr))]
        [Display(Name = "TaskName", ResourceType = typeof(ResourceScr))]
        public string Name { get; set; }

        [RegularExpression(@"[A-Za-zА-Яа-яА-Яа-я0-9,:._()\-\s\""]+",
        ErrorMessageResourceName = "ErrorRegulExpr", ErrorMessageResourceType = typeof(ResourceScr))]
        [StringLength(700, ErrorMessageResourceName = "ErrorStrLen", ErrorMessageResourceType = typeof(ResourceScr))]
        [Display(Name = "TaskDescription", ResourceType = typeof(ResourceScr))]
        public string Description { get; set; }

        [Display(Name = "TaskPriority", ResourceType = typeof(ResourceScr))]
        public Priority Priority { get; set; }
   
        
        [Display(Name = "TaskComplexity", ResourceType = typeof(ResourceScr))]
        public Complexity Complexity { get; set; }


        [Display(Name = "TaskEstimation", ResourceType = typeof(ResourceScr))]
        public Int64 EstimationTicks { get; set; }

        [NotMapped]
        [Display(Name = "TaskEstimation", ResourceType = typeof(ResourceScr))]
        public TimeSpan Estimation
        {
            get { return TimeSpan.FromTicks(EstimationTicks); }
            set { EstimationTicks = value.Ticks; }
        }

        public Int64 SpentTimeTicks { get; set; }
        [NotMapped]
        public TimeSpan SpentTime { 
            get { return TimeSpan.FromTicks(SpentTimeTicks); }
            set { SpentTimeTicks = value.Ticks; }
        }

        [NotMapped]
        [Display(Name = "Readiness", ResourceType = typeof(ResourceScr))]
        public double Readiness
        {
            get { return EstimationTicks == 0 ? 0 : Math.Min(SpentTimeTicks /(double)EstimationTicks * 100.0, 100.0); }
        }

        [Display(Name = "TaskDueDate", ResourceType = typeof(ResourceScr))]
        public DateTime? DesiredDueDate { get; set; }

        [Display(Name = "TaskDeadline", ResourceType = typeof(ResourceScr))]        
        public DateTime? Deadline { get; set; }

        [Display(Name = "TaskStartDate", ResourceType = typeof(ResourceScr))]
        public DateTime? DesiredStartDate { get; set; }
             
        public DateTime? CompletedOn { get; set; }
        public Guid UserId { get; set; }

        [Display(Name = "TaskProject", ResourceType = typeof(ResourceScr))]
        public Guid ProjectId { get; set; }

        [Display(Name = "TaskParentTask", ResourceType = typeof(ResourceScr))]
        public Guid? ParentTaskId { get; set; }

        [JsonIgnore]
        public virtual Project Project { get; set; }
        [JsonIgnore]
        public virtual Task ParentTask { get; set; }
        [JsonIgnore]
        public virtual ICollection<TaskCategory> TaskCategories { get; set; }
        [JsonIgnore]
        public virtual ICollection<Attachment> Attachments { get; set; }
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }

        [JsonIgnore]
        public virtual ICollection<Routine> Routines { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Task> ChildTasks { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; } 
    }
}
