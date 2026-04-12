using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ClienteApi.Models;
using ClienteApi.Middlewares;
using ClienteApi.Services;
using ClienteApi.Managers;
using ClienteApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IClientManager, ClientManager>();

var app = builder.Build();

// Centralized exceptions middleware usage
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();