using System.Text.RegularExpressions;

namespace ClienteDesktop.Helpers
{
    public static class Validator
    {
        #region User Data Validation
        // VALIDATE USER DATA BY USING REGULAR EXPRESSIONS (REGEX)
        
        public static bool IsValidDni(string dni)
        {
            // Validates DNI format: 8 numbers + 1 char
            if (string.IsNullOrWhiteSpace(dni)) return false;
            return Regex.IsMatch(dni.ToUpper(), @"^[0-9]{8}[A-Z]$");
        }

        public static bool IsValidEmail(string email)
        {
            // Validates email format: name@domain.extension
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool IsValidPhone(string phone)
        {
            // Validates phone has exactly 9 numbers
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone, @"^[0-9]{9}$");
        }
        #endregion
    }
}