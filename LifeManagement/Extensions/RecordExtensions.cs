using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LifeManagement.Models.DB;
using LifeManagement.Resources;

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
    }
}