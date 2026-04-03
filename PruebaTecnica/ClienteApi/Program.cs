using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ClienteApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// O mucho mejor, si tienes el paquete instalado (opción recomendada):
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

// 1. RUTA CORREGIDA: Apunta directamente a la carpeta del escritorio
string filePath = @"C:\Users\User\Desktop\Wolters-Kluwer\PruebaTecnica\ClienteDesktop\clients_store.json";

// Helper para leer el archivo
List<Client> GetClients()
{
    // Requisito: Asegurar existencia
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

// GET /clientes: devuelve el listado completo 
app.MapGet("/clientes", () => Results.Ok(GetClients()));

// GET /clientes/{dni}: devuelve un cliente concreto (404 si no existe)
app.MapGet("/clientes/{dni}", (string dni) =>
{
    var client = GetClients().FirstOrDefault(c => c.DNI.Equals(dni, StringComparison.OrdinalIgnoreCase));
    return client is not null ? Results.Ok(client) : Results.NotFound();
});

// POST /clientes: alta de cliente
app.MapPost("/clientes", async (HttpRequest request) =>
{
    // 1. Leemos el cuerpo de la petición como texto plano
    using var reader = new StreamReader(request.Body);
    string body = await reader.ReadToEndAsync();

    // 2. Usamos Newtonsoft.Json explícitamente para deserializar (esto sí leerá tus etiquetas [JsonProperty])
    var newClient = JsonConvert.DeserializeObject<Client>(body);

    // 3. Validaciones de negocio
    if (newClient == null || string.IsNullOrEmpty(newClient.DNI)) 
        return Results.BadRequest("Datos inválidos o DNI vacío");

    var clients = GetClients();
    
    // Comprobar si el DNI ya existe (ignora mayúsculas/minúsculas)
    if (clients.Any(c => c.DNI.Equals(newClient.DNI, StringComparison.OrdinalIgnoreCase))) 
        return Results.BadRequest("El DNI ya existe");

    // 4. Guardar
    clients.Add(newClient);
    SaveClients(clients);
    
    return Results.Created($"/clientes/{newClient.DNI}", newClient);
});

// DELETE /clientes/{dni}: elimina un cliente (204 si elimina, 404 si no existe)
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