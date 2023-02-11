using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for logger.
    /// </summary>
    public static class LoggerHelpers
    {
        /// <summary>
        /// Logs the system activity to specified directory.
        /// </summary>
        public static void Log(string text)
        {
            string fileName = string.Format("LogFile_{0}.txt", DateTime.Now.Date.ToShortDateString().Replace("/", "-"));
            var fullPath = Path.Combine(HostingEnvironment.MapPath("~/SystemLogs"), fileName);
            if (!File.Exists(fullPath))
            {
                using (StreamWriter sw = File.CreateText(fullPath))
                    sw.WriteLine(text);
            }
            else
            {
                using (StreamWriter sw = File.AppendText(fullPath))
                    sw.WriteLine(text);
            }
        }
    }
}