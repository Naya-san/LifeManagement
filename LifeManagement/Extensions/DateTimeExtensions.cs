using System;

namespace LifeManagement.Extensions
{
    public static class DateTimeExtensions
    {
        public static String toTimeFormat(this DateTime? dateTime)
        {
            String s = "";
            if (dateTime == null)
            {
                return "00:00";
            }
            if (dateTime.Value.Hour < 10)
            {
                s += "0";
            }
            s += dateTime.Value.Hour + ":";
            if (dateTime.Value.Minute < 10)
            {
                s += "0";
            }
            s += dateTime.Value.Minute;

            return s;
        }
    }
}