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
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using LifeManagement.Resources;

namespace LifeManagement.ObsoleteModels
{
    public class Project : ISynchronizableEntity
    {
        public virtual Guid Id { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(ResourceScr))]
        [StringLength(25, ErrorMessageResourceName = "ErrorStrLen", ErrorMessageResourceType = typeof(ResourceScr))]
        [RegularExpression(@"[A-Za-zА-Яа-яА-Яа-я0-9,:._\-()\s\""]+",
        ErrorMessageResourceName = "ErrorRegulExpr", ErrorMessageResourceType = typeof(ResourceScr))]
        [Display(Name = "name", ResourceType = typeof(ResourceScr))]
        public string Name { get; set; }
        public Guid UserId { get; set; }

        [NotMapped]
        [Display(Name = "Path", ResourceType = typeof(ResourceScr))]
        public String Path
        {
             get{ return String.Concat(ParentProject!=null? ParentProject.Path:"", "\\", Name); }
        }


        [Display(Name = "ParentProject", ResourceType = typeof(ResourceScr))]
        public Guid? ParentProjectId { get; set; }

        public virtual Project ParentProject { get; set; }
        public virtual ICollection<Project> ChildProjects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }

        public void Delite(LifeManagementContext db)
        {
            if (IsDeleted)
            {
                return;
            }
            IsDeleted = true;
            if (ChildProjects != null)
            {
                foreach (var child in ChildProjects)
                {
                    child.Delite(db);
                }
            }
           
            if (Tasks != null)
            {
                foreach (var child in Tasks)
                {
                    child.Delite(db);
                } 
            }
            UpdatedOn = DateTime.UtcNow;
            db.Entry(this).State = EntityState.Modified;
        }
    }
}
