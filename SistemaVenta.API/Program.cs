using SistemaVenta.API.Middleware;
using SistemaVenta.IOC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyeccion de Dependencias
builder.Services.DependencieInjection(builder.Configuration);

// Configuraci�n de CORS
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

// Configure the HTTP request pipeline.

// Configuraci�n Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("NuevaPolitica");

// Middleware de verificaci�n estado usuario activo

app.UseMiddleware<VerificarEstadoMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();
