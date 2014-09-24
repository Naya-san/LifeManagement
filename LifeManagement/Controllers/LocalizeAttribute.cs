using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;


namespace LifeManagement.Controllers
{
    public class LocalizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // HttpRequest request = HttpContext.Current.Request;
            //if (request.UserLanguages != null)
            //{
            //    string culture = request.UserLanguages[0];

            //    CultureInfo cultureInfo = new CultureInfo(culture);
            //    Thread.CurrentThread.CurrentCulture = cultureInfo;
            //    Thread.CurrentThread.CurrentUICulture = cultureInfo;
            //}

            //base.OnActionExecuting(filterContext);
            string culture = (filterContext.HttpContext.Session["culture"] != null)
              ? filterContext.HttpContext.Session["culture"].ToString()
              : "ru-ru";

            CultureInfo cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            base.OnActionExecuting(filterContext);
        }
    }
}