using ClienteApi.Models;

namespace ClienteApi.Services
{
    public class ClientService : IClientService {
        #region Vars
        private readonly string _path = "clients.json";
        #endregion

        #region Public Methods
        public async Task<List<Client>> GetClients()
        {
            return new List<Client>();
        }
        public async Task SaveClients(List<Client> clients)
        {
            
        }
        #endregion
    }
}