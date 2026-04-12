using Microsoft.AspNetCore.Mvc;
using ClienteApi.Models;
using ClienteApi.Managers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase {
    #region Vars
    private readonly IClientManager _manager;
    #endregion

    #region Constructors
    public ClientsController(IClientManager manager) { // Inyección del manager
        _manager = manager;
    }
    #endregion

    #region Endpoints
    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        return Ok(await _manager.GetAllValidClients());
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient(Client client) {
        var success = await _manager.AddClient(client);
        return success
            ? Ok()
            : BadRequest("El cliente ya existe o es inválido.");
    }
    #endregion
}