using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ClienteApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Requisito: Ruta al fichero JSON
string filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ClienteDesktop", "bin", "Debug", "net8.0-windows", "clients_store.json");

// Helper para leer el archivo
List<Client> GetClients()
{
    if (!File.Exists(filePath)) return new List<Client>();
    var json = File.ReadAllText(filePath);
    return JsonConvert.DeserializeObject<List<Client>>(json) ?? new List<Client>();
}

// Helper para guardar el archivo
void SaveClients(List<Client> clients) 
{
    var json = JsonConvert.SerializeObject(clients, Formatting.Indented);
    File.WriteAllText(filePath, json);
}

// --- ENDPOINTS REQUERIDOS ---

// GET /clientes: Listado completo
app.MapGet("/clientes", () => Results.Ok(GetClients()));

// GET /clientes/{dni}: Buscar por DNI
app.MapGet("/clientes/{dni}", (string dni) =>
{
    var client = GetClients().FirstOrDefault(c => c.DNI.Equals(dni, StringComparison.OrdinalIgnoreCase));
    return client is not null ? Results.Ok(client) : Results.NotFound();
});

// POST /clientes: Alta de cliente
app.MapPost("/clientes", ([FromBody] Client newClient) =>
{
    if (string.IsNullOrEmpty(newClient.DNI)) return Results.BadRequest("DNI no puede estar vacío");

    var clients = GetClients();
    if (clients.Any(c => c.DNI == newClient.DNI)) return Results.BadRequest("El DNI ya existe");

    clients.Add(newClient);
    SaveClients(clients);
    return Results.Created($"/clientes/{newClient.DNI}", newClient);
});

// DELETE /clientes/{dni}: Borrar cliente
app.MapDelete("/clientes/{dni}", (string dni) =>
{
    var clients = GetClients();
    var client = clients.FirstOrDefault(c => c.DNI.Equals(dni, StringComparison.OrdinalIgnoreCase));
    
    if (client is null) return Results.NotFound();

    clients.Remove(client);
    SaveClients(clients);
    return Results.NoContent(); // 204 No Content
});

app.Run();