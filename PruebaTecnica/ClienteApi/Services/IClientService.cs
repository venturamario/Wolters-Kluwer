using ClienteApi.Models;

namespace ClienteApi.Services
{
    public interface IClientService {
        Task<List<Client>> GetClients();
        Task SaveClients(List<Client> clients);
    }
}