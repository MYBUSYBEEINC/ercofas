using ERCOFAS.Models;
using PasswordGenerator;
using System;
using System.Linq;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for code generator.
    /// </summary>
    public static class CodeGenerator
    {
        private static readonly DefaultDBContext _db = new DefaultDBContext();

        /// <summary>
        /// Generates an ER number based on the last series.
        /// </summary>
        public static string GenerateERNumber()
        {
            string year = DateTime.Now.Year.ToString();
            string monthDay = string.Format("{0}{1}", DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString());
            var regWithERNumbers = _db.PreRegistration.Where(x => x.ERNumber != null).OrderBy(x => x.Id).ToList();
            int lastSeries = regWithERNumbers.Count > 0 ? (int)regWithERNumbers.LastOrDefault().Id : 1;

            return string.Format("{0}-{1}-{2}", year, monthDay, lastSeries); 
        }

        /// <summary>
        /// Generates an randomized otp code based on the number of digits.
        /// </summary>
        /// <param name="digits">The number of digits.</param>
        public static string GenerateOneTimePassword(int digits)
        {
            var password = new Password(digits).IncludeNumeric();
            return password.Next();
        }
    }
}