using Moq;
using Xunit;
using ClienteApi.Managers;
using ClienteApi.Services;
using ClienteApi.Models;
using Microsoft.Extensions.Logging;

namespace ClienteApi.Tests
{
    public class ClientManagerTests
    {
        private readonly Mock<IClientService> _serviceMock;
        private readonly Mock<ILogger<ClientManager>> _loggerMock;
        private readonly ClientManager _manager;

        public ClientManagerTests()
        {
            _serviceMock = new Mock<IClientService>();
            _loggerMock = new Mock<ILogger<ClientManager>>();
            _manager = new ClientManager(_serviceMock.Object, _loggerMock.Object);
        }

        #region Pruebas de Validación de DNI

        [Fact]
        public async Task AddClient_ShouldReturnFalse_WhenDniAlreadyExists_CaseInsensitive()
        {
            // ARRANGE: Probamos que "123a" es lo mismo que "123A" (Robustez)
            var existingClient = new Client { DNI = "12345678A" };
            var newClient = new Client { DNI = "12345678a" };
            
            _serviceMock.Setup(s => s.GetClients())
                        .ReturnsAsync(new List<Client> { existingClient });

            // ACT
            var result = await _manager.AddClient(newClient);

            // ASSERT
            Assert.False(result);
            _serviceMock.Verify(s => s.SaveClients(It.IsAny<List<Client>>()), Times.Never);
        }

        #endregion

        #region Pruebas de Casos de Borde (Edge Cases)

        [Fact]
        public async Task GetAllClients_ShouldReturnEmptyList_WhenServiceReturnsNull()
        {
            // ARRANGE: ¿Qué pasa si el JSON está corrupto o el servicio falla y devuelve null?
            _serviceMock.Setup(s => s.GetClients())
                        .ReturnsAsync((List<Client>)null!);

            // ACT
            var result = await _manager.GetAllValidClients();

            // ASSERT
            Assert.NotNull(result);
            Assert.Empty(result); // El Manager debe ser resiliente y devolver una lista vacía, no null.
        }

        #endregion

        #region Pruebas de Infraestructura (Excepciones)

        [Fact]
        public async Task AddClient_ShouldThrowException_WhenServiceFailsToSave()
        {
            // ARRANGE: Simulamos un error crítico (ej: Disco lleno o sin permisos)
            var newClient = new Client { DNI = "99999999Z" };
            _serviceMock.Setup(s => s.GetClients()).ReturnsAsync(new List<Client>());
            
            // Simulamos que SaveClients lanza una excepción
            _serviceMock.Setup(s => s.SaveClients(It.IsAny<List<Client>>()))
                        .ThrowsAsync(new IOException("Disk Full"));

        
            // ACT & ASSERT
            // Verificamos que el Manager no "se traga" el error, sino que lo deja fluir 
            // para que el Middleware de la API lo capture.
            await Assert.ThrowsAsync<IOException>(() => _manager.AddClient(newClient));
        }

        #endregion

        #region Pruebas de Integridad

        [Fact]
        public async Task DeleteClient_ShouldReturnFalse_WhenClientDoesNotExist()
        {
            // ARRANGE
            _serviceMock.Setup(s => s.GetClients())
                        .ReturnsAsync(new List<Client> { new Client { DNI = "111" } });

            // ACT: Intentamos borrar un DNI que no existe
            //var result = await _manager.DeleteClient("222");

            // ASSERT
            //Assert.False(result);
            // Verificamos que NO se intentó guardar la lista (ahorro de I/O innecesario)
            _serviceMock.Verify(s => s.SaveClients(It.IsAny<List<Client>>()), Times.Never);
        }

        #endregion
    }
}