using System;
using System.Configuration;
using System.Globalization;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for the date.
    /// </summary>
    public static class DateHelpers
    {
        /// <summary>
        /// Gets the system local time zone date & time.
        /// </summary>
        public static DateTime GetSystemTimeZoneDateTimeNow()
        {
            string timeZone = ConfigurationManager.AppSettings["timeZone"].ToString();
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, timeZone); ;
        }

        // <summary>
        /// Gets the iso utc date & time.
        /// </summary>
        public static string GetIsoUtcNow()
        {
            return DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}