using Newtonsoft.Json;
using System;

namespace ClienteDesktop.Models
{
    public class Client
    {
        [JsonProperty("dni")]
        public string DNI { get; set; }

        [JsonProperty("nombre")]
        public string FirstName { get; set; }

        [JsonProperty("apellidos")]
        public string LastName { get; set; }

        [JsonProperty("fechaNacimiento")]
        public DateTime BirthDate { get; set; }

        [JsonProperty("telefono")]
        public string Phone { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}