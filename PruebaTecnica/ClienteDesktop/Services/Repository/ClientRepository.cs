using Newtonsoft.Json;
using ClienteDesktop.Models;

// Centraliza la lecutra y escritura al JSON
namespace ClienteDesktop.Repository.Services 
{
    public class ClientRepository
    {
        private readonly string _path = @"C:\Users\User\Desktop\Wolters-Kluwer\PruebaTecnica\ClienteDesktop\clients_store.json";

        public List<Client> GetAll()
        {
            if (!File.Exists(_path)) return new List<Client>();
            var json = File.ReadAllText(_path);
            return JsonConvert.DeserializeObject<List<Client>>(json) ?? new List<Client>();
        }

        public void SaveAll(IEnumerable<Client> clients)
        {
            var json = JsonConvert.SerializeObject(clients, Formatting.Indented);
            File.WriteAllText(_path, json);
        }
    }
}