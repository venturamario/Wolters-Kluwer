using ClienteApi.Models;
using ClienteApi.Services;

namespace ClienteApi.Managers
{
    public class ClientManager : IClientManager {
        #region Vars
        private readonly IClientService _service;
        #endregion

        #region Constructors
        public ClientManager(IClientService service) {
            _service = service;
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddClient(Client newClient) {
            var clients = await _service.GetClients();
            if (clients.Any(c => c.DNI == newClient.DNI))
            {
                return false;
            }
            
            clients.Add(newClient);
            await _service.SaveClients(clients);
            return true;
        }
        
        public async Task<List<Client>> GetAllValidClients()
        {
            return await _service.GetClients();
        }
        #endregion
    }
}