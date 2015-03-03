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
using System.Web.Mvc;
using LifeManagement.Enums;
using LifeManagement.Resources;
using LifeManagement.Extensions;

namespace LifeManagement.Models.DB
{
    public class Task : Record
    {
        [Display(Name = "TaskProject", ResourceType = typeof(ResourceScr))]
        public Guid ProjectId { get; set; }

         [Display(Name = "CompletedOn", ResourceType = typeof(ResourceScr))]
        public DateTime? CompletedOn { get; set; }

        [Display(Name = "TaskComplexity", ResourceType = typeof(ResourceScr))]
        public Complexity Complexity { get; set; }

         [Display(Name = "CompleteLevel", ResourceType = typeof(ResourceScr))]
        public int CompleteLevel { get; set; }

        public virtual Project Project { get; set; }
        public override bool IsTimeValid(ModelStateDictionary modelState, AlertPosition alert)
        {
            if (!StartDate.HasValue && !EndDate.HasValue)
            {
                if (alert == AlertPosition.None)
                {
                    return true;
                }
                modelState.AddModelError("StartDate", ResourceScr.ErrorAlert);
                return false;
            }
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (EndDate.Value > StartDate.Value)
                {
                    return true;
                }
                modelState.AddModelError("EndDate", ResourceScr.ErrorDateOrder);
                return false;
            }
            return true;
        }

        public string ConvertTimeToNice(bool withComplete)
        {
            if (withComplete && CompletedOn.HasValue)
            {
                return ResourceScr.CompletedOn + " " + CompletedOn.ToString();
            }
            return this.ConvertTimeToNice();
        }
    }
}
