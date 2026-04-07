using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ClienteApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddControllers().AddNewtonsoftJson();
var app = builder.Build();

string filePath = @"C:\Users\User\Desktop\Wolters-Kluwer\PruebaTecnica\ClienteDesktop\clients_store.json";

List<Client> GetClients()
{
    // Requisito: Asegurar existencia
    if (!File.Exists(filePath)) return new List<Client>();
    var json = File.ReadAllText(filePath);
    return JsonConvert.DeserializeObject<List<Client>>(json) ?? new List<Client>();
}

void SaveClients(List<Client> clients) 
{
    var json = JsonConvert.SerializeObject(clients, Formatting.Indented);
    File.WriteAllText(filePath, json);
}

app.MapGet("/clientes", () => Results.Ok(GetClients()));

app.MapGet("/clientes/{dni}", (string dni) =>
{
    var client = GetClients().FirstOrDefault(c => c.DNI.Equals(dni, StringComparison.OrdinalIgnoreCase));
    return client is not null ? Results.Ok(client) : Results.NotFound();
});

app.MapPost("/clientes", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    string body = await reader.ReadToEndAsync();

    var newClient = JsonConvert.DeserializeObject<Client>(body);

    if (newClient == null || string.IsNullOrEmpty(newClient.DNI))
    {
        return Results.BadRequest("Datos inválidos o DNI vacío");
    }

    var clients = GetClients();
    
    // Comprobar si el DNI ya existe (ignora mayúsculas/minúsculas)
    if (clients.Any(c => c.DNI.Equals(newClient.DNI, StringComparison.OrdinalIgnoreCase))) 
        return Results.BadRequest("El DNI ya existe");

    clients.Add(newClient);
    SaveClients(clients);
    
    return Results.Created($"/clientes/{newClient.DNI}", newClient);
});

app.MapDelete("/clientes/{dni}", (string dni) =>
{
    var clients = GetClients();
    var client = clients.FirstOrDefault(c => c.DNI.Equals(dni, StringComparison.OrdinalIgnoreCase));
    
    if (client is null) return Results.NotFound();

    clients.Remove(client);
    SaveClients(clients);
    return Results.NoContent();     // 204
});

app.Run();