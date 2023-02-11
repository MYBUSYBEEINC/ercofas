using ERCOFAS.Models;
using PasswordGenerator;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for password.
    /// </summary>
    public static class PasswordHelpers
    {
        private static readonly DefaultDBContext _db = new DefaultDBContext();

        /// <summary>
        /// Generates a randomized password based on the settings.
        /// </summary>
        public static string GeneratePassword()
        {
            var settings = _db.Settings.FirstOrDefault(x => x.Id == "FA85FB3A-2A1E-47F8-9B76-6536F6A95ABB");

            if (settings == null)
                return string.Empty;

            var password = new Password(settings.MinPasswordLength).IncludeLowercase().IncludeUppercase()
                .IncludeNumeric().IncludeSpecial("[]{}^_=");

            return password.Next();
        }

        /// <summary>
        /// Hashed the password.
        /// <param name="password">The password.</param>
        /// </summary>
        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;

            if (password == null)
                return string.Empty;

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);

            return Convert.ToBase64String(dst);
        }
    }
}