using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LifeManagement.Models.DB;
using LifeManagement.Resources;
using WebGrease.Css.Ast.Selectors;

namespace LifeManagement.Extensions
{
    public static class RecordExtensions
    {
        public static string ConvertTimeToNice(this Record record)
        {
            if (!(record.EndDate.HasValue || record.StartDate.HasValue))
            {
                return ResourceScr.NoDueDate;
            }
            
            if (record.EndDate.HasValue && record.StartDate.HasValue &&
                record.StartDate.Value.Date == record.EndDate.Value.Date)
            {
                return String.Concat(ResourceScr.from, " ", record.StartDate.Value.ToString("t"), " ", ResourceScr.till, " ", record.EndDate.Value.ToString("t"), " ", record.EndDate.Value.ToString("dd MMMM yyyy"));
            }
            string res = "";
            if (record.StartDate.HasValue)
            {
                res = String.Concat(ResourceScr.from, " ", record.StartDate.Value.ToString("g"), " ");
            }

            if (record.EndDate.HasValue)
            {
                res += String.Concat(ResourceScr.till, " ", record.EndDate.Value.ToString("g"));
            }

            return res;
        }
        //public static bool IsTimeValidSimple(this Record record, ModelStateDictionary modelState)
        //{
        //    if (!record.StartDate.HasValue && !record.EndDate.HasValue)
        //    {
        //        if (record is Task)
        //        {
        //            return true;
        //        }
        //        modelState.AddModelError("EndDate", ResourceScr.ErrorRequired);
        //        modelState.AddModelError("StartDate", ResourceScr.ErrorRequired);
        //        return false;
        //    }
        //    if (record.StartDate.HasValue && record.EndDate.HasValue)
        //    {
        //        if (record.EndDate.Value > record.StartDate.Value)
        //        {
        //            //if (record is Event &&  ((Event)record).StopRepeatDate.HasValue)
        //            //{
        //            //    ((Event)record).StopRepeatDate.HasValue
        //            //}
        //            return true;
        //        }
        //        modelState.AddModelError("EndDate", ResourceScr.ErrorDateOrder);
        //        return false;
        //    }
           
        //    return true;
        //}
        //public static bool IsTimeValid(this Record record, ModelStateDictionary modelState)
        //{
        //    if (!record.StartDate.HasValue && !record.EndDate.HasValue)
        //    {
        //        if (record is Task)
        //        {
        //            return true;
        //        }
        //        modelState.AddModelError("EndDate", ResourceScr.ErrorRequired);
        //        modelState.AddModelError("StartDate", ResourceScr.ErrorRequired);
        //        return false;
        //    }
        //    var today = DateTime.UtcNow.Date;
        //    if (record.StartDate.HasValue && record.EndDate.HasValue)
        //    {
        //        if (record.StartDate.Value >= today && record.EndDate.Value > record.StartDate.Value)
        //        {
        //            return true;
        //        }
        //        modelState.AddModelError("EndDate", ResourceScr.ErrorDateOrder);
        //        return false;
        //    }
        //    if (record.StartDate.HasValue && record.StartDate.Value >= today)
        //    {
        //        return true;
        //    }
        //    if (record.EndDate.HasValue && record.EndDate.Value >= today)
        //    {
        //        return true;
        //    }
        //    modelState.AddModelError("EndDate", ResourceScr.ErrorDateOrder);
        //    return false;
        //}
    }

}