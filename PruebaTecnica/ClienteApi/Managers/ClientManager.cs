using ClienteApi.Models;
using ClienteApi.Services;
using ClienteApi.Managers;
using Microsoft.Extensions.Logging;

public class ClientManager : IClientManager {
    #region Vars
    private readonly IClientService _service;
    private readonly ILogger<ClientManager> _logger;

    public ClientManager(IClientService service, ILogger<ClientManager> logger) {
        _service = service;
        _logger = logger;
    }
    #endregion

    #region Public Methods
    public async Task<bool> AddClient(Client newClient) {
        try {
            var clients = await _service.GetClients();
            if (clients.Any(c => c.DNI == newClient.DNI))
            {
                return false;
            }

            clients.Add(newClient);
            await _service.SaveClients(clients);
            return true;

        } catch (Exception ex) {
            _logger.LogError(ex, "Error fatal al añadir el cliente con DNI {DNI}", newClient.DNI);
            throw new Exception("Error interno al procesar el almacenamiento."); 
        }
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
            _logger.LogError(ex, "Error fatal al obtener los clientes: {Message}", ex.Message);
            throw new Exception("Error fatal al obtener los clientes."); 
        }
    }
    #endregion
}