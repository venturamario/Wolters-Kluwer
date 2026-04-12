using System.Text.Json;
using ClienteApi.Models;
using FileStream = System.IO.FileStream;

namespace ClienteApi.Services
{
    public class ClientService : IClientService {
        #region Vars
        private readonly string _path = "clients.json";
        private FileStream? openStream;
        private FileStream? createStream;
        #endregion

        #region Public Methods
        public async Task<List<Client>> GetClients()
        {
            if (!File.Exists(_path))
            {
                return new List<Client>();
            }

            openStream = File.OpenRead(_path);
            return await JsonSerializer.DeserializeAsync<List<Client>>(openStream) ?? new List<Client>();
        }
        
        public async Task SaveClients(List<Client> clients)
        {
            createStream = File.Create(_path);
            await JsonSerializer.SerializeAsync(createStream, clients, new JsonSerializerOptions { WriteIndented = true });
        }
        #endregion
    }
}