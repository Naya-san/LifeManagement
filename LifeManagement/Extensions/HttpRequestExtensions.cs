using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.Extensions
{
    public static class HttpRequestExtensions
    {
        public static int GetTimeZoneOffsetMinutes(this HttpRequest request)
        {
            try
            {
                return (-1) * int.Parse(request.Cookies["timeZoneOffset"].Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static DateTime GetUtcFromUserLocalTime(this HttpRequest request, DateTime dateTime)
        {
            int timeZoneOffsetMinutes = request.GetTimeZoneOffsetMinutes();
            if (dateTime > new DateTime(1950, 1, 1) && dateTime < new DateTime(2100, 1, 1))
            {
                return dateTime.AddMinutes((-1) * timeZoneOffsetMinutes);
            }
            return dateTime;
        }
        public static DateTime GetUserLocalTimeFromUtc(this HttpRequest request, DateTime dt)
        {
            int timeZoneOffsetMinutes = request.GetTimeZoneOffsetMinutes();
            if (dt > new DateTime(1950, 1, 1) && dt < new DateTime(2100, 1, 1))
            {
                return dt.AddMinutes(timeZoneOffsetMinutes);
            }
                return dt;
        }

        public static DateTime? GetUtcFromUserLocalTime(this HttpRequest request, DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                DateTime dt = new DateTime(dateTime.Value.Ticks);
                return request.GetUtcFromUserLocalTime(dt); 
            }
            return dateTime;
        }
        public static DateTime? GetUserLocalTimeFromUtc(this HttpRequest request, DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                DateTime dt = new DateTime(dateTime.Value.Ticks);
                return request.GetUserLocalTimeFromUtc(dt);
                
            }
            return dateTime;

        }
    }
}