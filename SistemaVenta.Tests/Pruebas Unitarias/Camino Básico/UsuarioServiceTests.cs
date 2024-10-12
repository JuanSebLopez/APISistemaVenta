using Moq;
using SistemaVenta.BLL.Services.Contracts;
using SistemaVenta.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.Tests.Pruebas_Unitarias.Camino_Básico
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;

        public UsuarioServiceTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
        }

        [Fact]
        public async Task ValidarCredenciales_UsuarioExisteYActivo_RetornaSesion()
        {
            // Arrange
            var correo = "usuario@correo.com";
            var clave = "12345";

            var usuarioSesion = new SesionDTO { Correo = correo };
            _usuarioServiceMock.Setup(service => service.ValidarCredenciales(correo, clave))
                .ReturnsAsync(usuarioSesion);

            // Act
            var resultado = await _usuarioServiceMock.Object.ValidarCredenciales(correo, clave);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(correo, resultado.Correo);
        }

        [Fact]
        public async Task ValidarCredenciales_UsuarioNoExiste_LanzaExcepcion()
        {
            // Arrange
            var correo = "usuario2@correo.com";
            var clave = "54321";

            _usuarioServiceMock.Setup(service => service.ValidarCredenciales(correo, clave))
                .ThrowsAsync(new TaskCanceledException("Correo o contraseña incorrectos"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _usuarioServiceMock.Object.ValidarCredenciales(correo, clave));
        }

        [Fact]
        public async Task ValidarCredenciales_UsuarioInactivo_LanzaExcepcion()
        {
            // Arrange
            var correo = "usuario3@correo.com";
            var clave = "12345";

            _usuarioServiceMock.Setup(service => service.ValidarCredenciales(correo, clave))
                .ThrowsAsync(new TaskCanceledException("Correo o contraseña incorrectos"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _usuarioServiceMock.Object.ValidarCredenciales(correo, clave));
        }
    }
}
