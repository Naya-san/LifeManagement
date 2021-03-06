﻿#region License
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

namespace LifeManagement.Models.DB
{
    public class Event : Record
    {
        public Guid GroupId { get; set; }
        public RepeatPosition RepeatPosition { get; set; }

        public DateTime? StopRepeatDate { get; set; }

        [Display(Name = "OnBackground", ResourceType = typeof(ResourceScr))]
        public bool OnBackground { set; get; }

        public override bool IsTimeValid(ModelStateDictionary modelState, AlertPosition alert)
        {
            if (!StartDate.HasValue)
            {
                modelState.AddModelError("StartDate", ResourceScr.ErrorRequired);
                return false;
            }
            if (!EndDate.HasValue)
            {
                modelState.AddModelError("EndDate", ResourceScr.ErrorRequired);
                return false;
            }

            if (EndDate.Value > StartDate.Value)
            {
                if (RepeatPosition != RepeatPosition.None)
                {
                    if (!StopRepeatDate.HasValue)
                    {
                        modelState.AddModelError("StopRepeatDate", ResourceScr.ErrorRequired);
                        return false;
                    }
                    if (StopRepeatDate.Value <= EndDate.Value)
                    {
                        modelState.AddModelError("StopRepeatDate", ResourceScr.ErrorDateStopOrder);
                        return false;
                    }
                }
                return true;
            }
            modelState.AddModelError("EndDate", ResourceScr.ErrorDateOrder);
            return false;
        }

        public override TimeSpan CalculateTimeLeft(UserSetting setting)
        {
            var dateNow = DateTime.UtcNow;
            if (dateNow > EndDate || !EndDate.HasValue || !StartDate.HasValue)
            {
                return new TimeSpan(0);
            }
            var timeSpan = EndDate.Value.Subtract(dateNow > StartDate ? dateNow : StartDate.Value);
            if (OnBackground)
            {
                timeSpan = new TimeSpan(timeSpan.Ticks*setting.ParallelismPercentage/100);
            }
            return timeSpan;
        }
        public TimeSpan CalculateTimeLeftInDay(UserSetting setting, DateTime date)
        {
            double minutesInDay = 60 * 24;
            if (date > EndDate || !EndDate.HasValue || !StartDate.HasValue)
            {
                return new TimeSpan(0);
            }
            var timeSpan = EndDate.Value.Subtract(date > StartDate ? date : StartDate.Value);
            if (timeSpan.TotalMinutes > minutesInDay)
            {
                timeSpan = TimeSpan.FromMinutes(minutesInDay);
            }
            if (OnBackground)
            {
                timeSpan = new TimeSpan(timeSpan.Ticks * setting.ParallelismPercentage / 100);
            }
            return timeSpan;
        }
    }
}
