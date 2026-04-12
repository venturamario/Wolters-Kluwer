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
    public ClientsController(IClientManager manager) {
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
    public async Task<IActionResult> CreateClient(Client client) 
    {
        try
        {
            var success = await _manager.AddClient(client);
            return success 
                ? CreatedAtAction(nameof(GetAllClients), new { id = client.DNI }, client) 
                : BadRequest("El cliente ya existe.");
        } 
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor.");
        }
    }
    #endregion
}