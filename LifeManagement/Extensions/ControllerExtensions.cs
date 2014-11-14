using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LifeManagement.Attributes;
using LifeManagement.Models;
using LifeManagement.Models.DB;
using Microsoft.AspNet.Identity;

namespace LifeManagement.Extensions
{
    public class ControllerExtensions : Controller
    {
        public ActionResult RedirectToPrevious()
        {
            try
            {
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Cabinet");
            }

        }
    }
}