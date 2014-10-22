using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace LifeManagement.Attributes
{
    public class LocalizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            /* var request = HttpContext.Current.Request;
             if (request.UserLanguages != null)
             {
                 var culture = request.UserLanguages[0];

                 var cultureInfo = new CultureInfo(culture);
                 Thread.CurrentThread.CurrentCulture = cultureInfo;
                 Thread.CurrentThread.CurrentUICulture = cultureInfo;
             }

             base.OnActionExecuting(filterContext);*/
             string culture = (filterContext.HttpContext.Session["culture"] != null)
               ? filterContext.HttpContext.Session["culture"].ToString()
               : "uk-ua";

             var cultureInfo = new CultureInfo(culture);
             Thread.CurrentThread.CurrentCulture = cultureInfo;
             Thread.CurrentThread.CurrentUICulture = cultureInfo;

             base.OnActionExecuting(filterContext);
        }
    }
}