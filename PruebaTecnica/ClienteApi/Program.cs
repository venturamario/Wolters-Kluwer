using ClienteApi.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string filePath = Path.Combine(AppContext.BaseDirectory, "data", "clients_store.json");

if (!Directory.Exists("data")) Directory.CreateDirectory("data");

// Helper para leer/escribir
List<Client> GetClients() {
    if (!File.Exists(filePath)) return new List<Client>();
    var json = File.ReadAllText(filePath);
    return JsonConvert.DeserializeObject<List<Client>>(json) ?? new List<Client>();
}

void SaveClients(List<Client> clients) => 
    File.WriteAllText(filePath, JsonConvert.SerializeObject(clients, Formatting.Indented));

app.MapGet("/clientes", () => GetClients()); // [cite: 20]

app.MapGet("/clientes/{dni}", (string dni) => {
    var client = GetClients().FirstOrDefault(c => c.DNI == dni);
    return client is not null
        ? Results.Ok(client)
        : Results.NotFound();
});

app.MapPost("/clientes", (Client newClient) => {
    var clients = GetClients();
    if (clients.Any(c => c.DNI == newClient.DNI))
    {
        return Results.BadRequest("DNI already exists");
    }
    clients.Add(newClient);
    SaveClients(clients);
    return Results.Created($"/clientes/{newClient.DNI}", newClient);
});

app.MapDelete("/clientes/{dni}", (string dni) => {
    var clients = GetClients();
    var client = clients.FirstOrDefault(c => c.DNI == dni);
    if (client is null)
    {
        return Results.NotFound();
    }
    clients.Remove(client);
    SaveClients(clients);
    return Results.NoContent();
});

app.Run();