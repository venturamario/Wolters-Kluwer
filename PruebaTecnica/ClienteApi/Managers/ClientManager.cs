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
            bool added = false;
            try
            {
                var clients = await _service.GetClients();
                if (clients.Any(c => c.DNI == newClient.DNI))
                {
                    return added;
                }
                
                clients.Add(newClient);
                await _service.SaveClients(clients);
                added = true;
                
            } catch (Exception ex)
            {
                Console.WriteLine("Error al añadir cliente: " + ex.Message);
                added = false;
            }
            return added;
        }
        
        public async Task<List<Client>> GetAllValidClients()
        {
            List<Client> validClients = new List<Client>();
            try
            {
                validClients = await _service.GetClients();
                return validClients;

            } catch (Exception ex)
            {
                Console.WriteLine("Error al obtener clientes: " + ex.Message);
                return validClients;
            }
        }
        #endregion
    }
}