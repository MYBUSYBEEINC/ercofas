using PasswordGenerator;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for code generator.
    /// </summary>
    public static class CodeGenerator
    {
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