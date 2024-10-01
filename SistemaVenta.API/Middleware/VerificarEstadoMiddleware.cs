using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;

namespace SistemaVenta.API.Middleware
{
    public class VerificarEstadoMiddleware
    {
        private readonly RequestDelegate _next;

        public VerificarEstadoMiddleware (RequestDelegate next) { _next = next; }

        public async Task Invoke(HttpContext context, DbventaContext dbContext)
        {
            var usuarioSesion = context.User.Identity.Name;
            Console.WriteLine("Middleware ejecutado para el usuario: " + usuarioSesion);

            var usuario = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.Correo == usuarioSesion);

            if (usuario != null && usuario.EsActivo.HasValue && usuario.EsActivo == false)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Tu cuenta ha sido desactivada.");
                return;
            }

            await _next(context);
        }
    }
}
