using System;

namespace LifeManagement.Extensions
{
    public static class DateTimeExtensions
    {
        public static String ToTimeFormat(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return "00:00";
            }
            return ToTimeFormat(dateTime.Value);
        }
        public static String ToTimeFormat(this DateTime dateTime)
        {
            String s = "";
            if (dateTime.Hour < 10)
            {
                s += "0";
            }
            s += dateTime.Hour + ":";
            if (dateTime.Minute < 10)
            {
                s += "0";
            }
            s += dateTime.Minute;

            return s;
        }
    }
}