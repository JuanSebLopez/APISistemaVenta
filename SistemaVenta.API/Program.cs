using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.IOC;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.IsProduction() ? "Production" : "Development";

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional:true)
    .AddEnvironmentVariables()
    .Build();

Console.WriteLine($"Entorno actual: {environment}");
Console.WriteLine($"Cadena de conexión: {configuration.GetConnectionString("cadenaSQL")}");

builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("cadenaSQL")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyeccion de Dependencias
builder.Services.DependencieInjection(builder.Configuration);

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica", app =>
    {
        app.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configuración Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("NuevaPolitica");

app.UseAuthorization();
app.MapControllers();

app.Run();
