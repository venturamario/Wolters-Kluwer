using System;

namespace ClienteDesktop.Models
{
    public class Client
    {
        public string DNI { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}