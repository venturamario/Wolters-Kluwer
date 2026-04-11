using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using ClienteDesktop.Models;

namespace ClienteDesktop.Services.Repository {
    public class ClientRepository {
        private readonly string _path = "clients_store.json";

        public List<Client> GetAll() {
            if (!File.Exists(_path))
            {
                return new List<Client>();
            }
            return JsonConvert.DeserializeObject<List<Client>>(File.ReadAllText(_path)) ?? new List<Client>();
        }

        public void SaveAll(IEnumerable<Client> clients)
        {
            File.WriteAllText(_path, JsonConvert.SerializeObject(clients, Formatting.Indented));
        }

    }
}