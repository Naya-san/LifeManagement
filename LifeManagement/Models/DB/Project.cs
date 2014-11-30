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
using System.Linq;
using LifeManagement.Resources;
using Microsoft.Ajax.Utilities;

namespace LifeManagement.Models.DB
{
    public class Project : IEntity
    {
        public Guid Id { get; set; }

        [Display(Name = "ParentProject", ResourceType = typeof(ResourceScr))]
        public Guid? ParentProjectId { get; set; }

        public string UserId { get; set; }


        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(ResourceScr))]
        [StringLength(25, ErrorMessageResourceName = "ErrorStrLen", ErrorMessageResourceType = typeof(ResourceScr))]
        [Display(Name = "Name", ResourceType = typeof(ResourceScr))]
        public string Name { get; set; }


        public virtual Project ParentProject { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Project> ChildProjects { get; private set; }
        public virtual ICollection<Task> Tasks { get; private set; }


        [NotMapped]
        [Display(Name = "Path", ResourceType = typeof(ResourceScr))]
        public String Path
        {
            get { return ParentProjectId == null ? Name : String.Concat(ParentProject.Path, "\\", Name); }
        }

        private bool CanHasLikeSonUp(Guid projectId)
        {
            if (Id == projectId)
            {
                return false;
            }
            if (ParentProjectId == null)
            {
                return true;
            }
            return ParentProject.CanHasLikeSonUp(projectId);
        }
        private bool CanHasLikeSonDown(Guid projectId)
        {
            if (Id == projectId)
            {
                return false;
            }
            if (ChildProjects == null || !ChildProjects.Any())
            {
                return true;
            }
            foreach (var childProject in ChildProjects)
            {
                if (!childProject.CanHasLikeSonDown(projectId))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanHasLikeSon(Guid projectId)
        {

            if (CanHasLikeSonUp(projectId))
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            const int maxLength = 30;
            if (Path.Length <= maxLength)
            {
                return Path;
            }
            var parts = Path.Split('\\');
            int length = Path.Length;
            string res = "";
            for (int i = 0; i < parts.Length; i++)
            {

                if (parts[i].Length > 4)
                {
                    res += parts[i][0] + "...";
                    length -= (parts[i].Length - 4);
                }
                else
                {
                    res += parts[i];
                }
                if (length <= maxLength)
                {
                    for (int j = i+1; j < parts.Length; j++)
                    {
                        res += "\\" + parts[j];
                    }
                    break;
                }
                res += "\\";
            }
            if (res.Length > maxLength)
            {
                res = "...\\" + Name;
            }
            return res;
        }


        public Project()
        {
            Id = Guid.NewGuid();
            ChildProjects = new List<Project>();
            Tasks = new List<Task>();
        }
    }
}
