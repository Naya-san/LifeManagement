using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace LifeManagement.Binder
{
    public class DateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("ru-ru");

            return value.ConvertTo(typeof(DateTime), cultureinfo);
        }
    }
    public class NullableDateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("ru-ru");
            return value == null
                ? null
                : value.ConvertTo(typeof(DateTime), cultureinfo);
        }
    }  
}