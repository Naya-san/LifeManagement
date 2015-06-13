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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LifeManagement.Enums;
using LifeManagement.Resources;
using System.Web.Mvc;
using Microsoft.Owin.Security.Provider;

namespace LifeManagement.Models.DB
{
    public abstract class Record : IEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }


        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(ResourceScr))]
        [StringLength(40, ErrorMessageResourceName = "ErrorStrLen", ErrorMessageResourceType = typeof(ResourceScr))]
        [Display(Name = "Name", ResourceType = typeof(ResourceScr))]
        public string Name { get; set; }
        [StringLength(700, ErrorMessageResourceName = "ErrorStrLen", ErrorMessageResourceType = typeof(ResourceScr))]
        [Display(Name = "TaskDescription", ResourceType = typeof(ResourceScr))]
        public string Note { get; set; }

        [Display(Name = "TaskStartDate", ResourceType = typeof(ResourceScr))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "TaskDueDate", ResourceType = typeof(ResourceScr))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Important", ResourceType = typeof(ResourceScr))]
        public bool IsImportant { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Tag> Tags { get; private set; }
        public virtual ICollection<Alert> Alerts { get; private set; }

        public virtual ICollection<ListForDay> ListForDays { get; set; }

        public abstract bool IsTimeValid(ModelStateDictionary modelState, AlertPosition alert);

        public abstract TimeSpan CalculateTimeLeft(UserSetting setting);

        protected Record()
        {
            Id = Guid.NewGuid();
            Alerts = new List<Alert>();
            Tags = new List<Tag>();
            ListForDays = new List<ListForDay>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Record))
            {
                return false;
            }
            var task = obj as Record;

            return task.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
