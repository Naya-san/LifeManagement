using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LifeManagement.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString DialogFormLink(this HtmlHelper htmlHelper, string linkText, string cssClass, string dialogContentUrl,
             string dialogTitle, string updateTargetId, string updateUrl, int width)
        {
            var builder = new TagBuilder("a");
            builder.SetInnerText(linkText);
            builder.AddCssClass(cssClass);
            builder.Attributes.Add("href", dialogContentUrl);
            builder.Attributes.Add("data-dialog-title", dialogTitle);
            builder.Attributes.Add("data-update-target-id", updateTargetId);
            builder.Attributes.Add("data-update-url", updateUrl);
            builder.Attributes.Add("data-dialog-width", width.ToString(CultureInfo.InvariantCulture));
            // Add a css class named dialogLink that will be
            // used to identify the anchor tag and to wire up
            // the jQuery functions
            builder.AddCssClass("dialogLink");

            return new MvcHtmlString(builder.ToString());
        
        }

        public static MvcHtmlString DialogFormLink(this HtmlHelper htmlHelper, string linkText, string cssClass, string dialogContentUrl,
     string dialogTitle, string updateTargetId, string updateUrl, int width, string HelpText)
        {
            var builder = new TagBuilder("a");
            builder.SetInnerText(linkText);
            builder.AddCssClass(cssClass);
            builder.Attributes.Add("href", dialogContentUrl);
            builder.Attributes.Add("data-dialog-title", dialogTitle);
            builder.Attributes.Add("data-update-target-id", updateTargetId);
            builder.Attributes.Add("data-update-url", updateUrl);
            builder.Attributes.Add("data-dialog-width", width.ToString(CultureInfo.InvariantCulture));
            builder.Attributes.Add("title", HelpText);
            // Add a css class named dialogLink that will be
            // used to identify the anchor tag and to wire up
            // the jQuery functions
            builder.AddCssClass("dialogLink");

            return new MvcHtmlString(builder.ToString());
        }
    }
}