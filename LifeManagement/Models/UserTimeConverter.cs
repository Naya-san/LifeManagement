using System;
using System.Web;

namespace LifeManagement.Models
{
    public static class UserTimeConverter
    {
        public static int GetTimeZoneOffsetMinutes(HttpRequest request)
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

        public static DateTime GetUtcFromUserLocalTime(Guid accountId, HttpRequest request, DateTime dt)
        {
            return GetUtcFromUserLocalTime(accountId, dt, GetTimeZoneOffsetMinutes(request));
        }

        public static DateTime GetUtcFromUserLocalTime(Guid accountId, DateTime dt, int timeZoneOffsetMinutes)
        {
            if (dt > new DateTime(1950, 1, 1) && dt < new DateTime(2100, 1, 1))
            {
                return dt.AddMinutes((-1) * timeZoneOffsetMinutes);
            }
            else
            {
                return dt;
            }
        }

        public static DateTime GetUserLocalTimeFromUtc(Guid accountId, HttpRequest request, DateTime dt)
        {
            return GetUserLocalTime(accountId, dt, GetTimeZoneOffsetMinutes(request));
        }


        public static DateTime GetUserLocalTime(Guid accountId, DateTime dt, int timeZoneOffsetMinutes)
        {
            if (dt > new DateTime(1950, 1, 1) && dt < new DateTime(2100, 1, 1))
            {
                return dt.AddMinutes(timeZoneOffsetMinutes);
            }
            else
            {
                return dt;
            }
        }

        public static DateTime LocalDateToUtc(DateTime dt)
        {
            return TimeZone.CurrentTimeZone.ToUniversalTime(dt);
        }

        public static DateTime LocalDateToUtc(object dt)
        {
            return LocalDateToUtc((DateTime)dt);
        }
    }
}