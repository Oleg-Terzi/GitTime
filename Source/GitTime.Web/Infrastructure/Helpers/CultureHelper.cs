using System;
using System.Globalization;

namespace GitTime.Web.Infrastructure.Helpers
{
    public static class CultureHelper
    {
        public static readonly CultureInfo Current = new CultureInfo("en-US");

        #region Formatting

        public static String Format(String text, params Object[] args)
        {
            return String.Format(Current, text, args);
        }

        public static String Format(DateTime date)
        {
            return Format(date, true, false);
        }

        public static String Format(DateTime date, Boolean displayTime)
        {
            return Format(date, true, displayTime);
        }

        public static String Format(DateTime date, Boolean showDate, Boolean displayTime)
        {
            String format = String.Empty;

            if (showDate)
                format += "MMM d, yyyy";

            if (displayTime)
            {
                if (format.Length > 0)
                    format += " ";

                format += "h:mm tt";
            }

            if (!String.IsNullOrEmpty(format) && date != DateTime.MinValue)
            {
                return date.ToString(format, Current);
            }

            return String.Empty;
        }

        #endregion
    }
}