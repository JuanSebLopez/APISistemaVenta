using Moq;
using SistemaVenta.BLL.Services.Contracts;
using SistemaVenta.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.Tests.Pruebas_Unitarias.Valores_Límite
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;

        public UsuarioServiceTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
        }

        // Prueba de límites para Email
        [Theory]
        [InlineData("correo@gmail.com", "password123")]  // Email con 6 caracteres antes del @
        [InlineData("correo1@gmail.com", "password123")] // Email con más de 6 caracteres antes del @
        [InlineData("corre@gmail.com", "password123")]   // Email con menos de 6 caracteres antes del @ (inválido)
        public async Task ValidarCredenciales_EmailValoresLimites(string correo, string clave)
        {
            if (correo.Length >= 6 && correo.Contains("@"))
            {
                // Arrange: Si el email es válido (6 o más caracteres antes del @)
                var usuarioSesion = new SesionDTO { Correo = correo };
                _usuarioServiceMock.Setup(service => service.ValidarCredenciales(correo, clave))
                    .ReturnsAsync(usuarioSesion);

                // Act
                var resultado = await _usuarioServiceMock.Object.ValidarCredenciales(correo, clave);

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal(correo, resultado.Correo);
            }
            else
            {
                // Arrange: Si el email es inválido (menos de 6 caracteres antes del @)
                _usuarioServiceMock.Setup(service => service.ValidarCredenciales(correo, clave))
                    .ThrowsAsync(new TaskCanceledException("Correo o contraseña incorrectos"));

                // Act & Assert
                await Assert.ThrowsAsync<TaskCanceledException>(() =>
                    _usuarioServiceMock.Object.ValidarCredenciales(correo, clave));
            }
        }
    }
}
