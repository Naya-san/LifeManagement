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
                res = String.Concat(ResourceScr.from, " ", record.StartDate.Value.ToString("dd.MM.yyyy HH:mm"), " ");
            }

            if (record.EndDate.HasValue)
            {
                res += String.Concat(ResourceScr.till, " ", record.EndDate.Value.ToString("dd.MM.yyyy HH:mm"));
            }

            return res;
        }
    }

}