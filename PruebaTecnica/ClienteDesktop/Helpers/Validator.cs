using System.Text.RegularExpressions;

namespace ClienteDesktop
{
    public static class DataValidator
    {
        // Valida DNI español: 8 números y una letra que corresponda al algoritmo
        public static bool IsValidDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) return false;
            return Regex.IsMatch(dni.ToUpper(), @"^[0-9]{8}[A-Z]$");
        }

        // Valida formato de email estándar
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // Comprueba formato usuario@dominio.extension
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Valida que el teléfono tenga 9 dígitos numéricos
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone, @"^[0-9]{9}$");
        }
    }   
}