using ClienteApi.Models;

namespace ClienteApi.Managers
{
    public interface IClientManager {
        Task<List<Client>> GetAllValidClients();
        Task<bool> AddClient(Client newClient);
    }
}