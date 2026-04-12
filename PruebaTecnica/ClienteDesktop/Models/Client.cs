using Newtonsoft.Json;
using System;

namespace ClienteDesktop.Models
{
    public class Client
    {
        [JsonProperty("dni")]
        public string DNI { get; set; } = string.Empty;

        [JsonProperty("nombre")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("apellidos")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("fechaNacimiento")]
        public DateTime BirthDate { get; set; }

        [JsonProperty("telefono")]
        public string Phone { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;
    }
}