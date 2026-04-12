using ClienteDesktop.Models;

namespace ClienteDesktop.Services.Repository
{
    public interface IClientRepository
    {
        List<Client> GetAll();
        void SaveAll(IEnumerable<Client> clients);
    }
}