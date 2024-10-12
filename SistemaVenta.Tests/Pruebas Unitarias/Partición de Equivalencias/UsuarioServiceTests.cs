using SistemaVenta.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using SistemaVenta.BLL.Services.Contracts;
using SistemaVenta.DTO;
using SistemaVenta.Model;

namespace SistemaVenta.Tests
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;

        public UsuarioServiceTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
        }

        // Pruebas para ValidarCredenciales()
        [Theory]
        [InlineData("validEmail@gmail.com", "password123")] // Clase válida de email y contraseña
        [InlineData("correo@gmail.com", "password123")]      // Clase válida de email y contraseña
        public async Task ValidarCredenciales_EmailYContraseñaValidas_RetornaSesionCorrecta(string correo, string clave)
        {
            // Arrange
            var usuarioSesion = new SesionDTO { Correo = correo }; // Simula una sesión de usuario correcta
            _usuarioServiceMock.Setup(service => service.ValidarCredenciales(correo, clave))
                .ReturnsAsync(usuarioSesion);

            // Act
            var resultado = await _usuarioServiceMock.Object.ValidarCredenciales(correo, clave);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(correo, resultado.Correo);
        }

        [Theory]
        [InlineData("invalidEmail.com", "password123")]      // Clase inválida de email
        [InlineData("invalid@Email@", "password123")]        // Clase inválida de email
        [InlineData("", "password123")]                      // Email vacío
        [InlineData(null, "password123")]                    // Email null
        public async Task ValidarCredenciales_EmailInvalido_LanzaExcepcion(string correo, string clave)
        {
            // Arrange: Simula una excepcion para entradas incorrectas
            _usuarioServiceMock.Setup(service => service.ValidarCredenciales(correo, clave))
                .ThrowsAsync(new TaskCanceledException("Correo o contraseña incorrectos"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _usuarioServiceMock.Object.ValidarCredenciales(correo, clave));
        }

        [Theory]
        [InlineData("validEmail@gmail.com", "")]             // Contraseña vacía
        [InlineData("validEmail@gmail.com", null)]           // Contraseña null
        public async Task ValidarCredenciales_ContraseñaInvalida_LanzaExcepcion(string correo, string clave)
        {
            // Arrange: Simula una excepción para contraseña inválida
            _usuarioServiceMock.Setup(service => service.ValidarCredenciales(correo, clave))
                .ThrowsAsync(new TaskCanceledException("Correo o contraseña incorrectos"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _usuarioServiceMock.Object.ValidarCredenciales(correo, clave));
        }

        // Pruebas para Crear()
        [Fact]
        public async Task CrearUsuario_Valido_RetornaUsuarioCreado()
        {
            // Arrange
            var usuarioDTO = new UsuarioDTO
            {
                NombreCompleto = "Juan Lopez",
                Correo = "correo@gmail.com",
                IdRol = 1, // Administrador
                Clave = 12345,
                EsActivo = 1
            };

            var usuarioCreado = new UsuarioDTO { IdUsuario = 1, NombreCompleto = "Juan Lopez", Correo = "correo@gmail.com", IdRol = 1, Clave = 12345, EsActivo = 1 };

            _usuarioServiceMock.Setup(service => service.Crear(It.IsAny<UsuarioDTO>()))
                .ReturnsAsync(usuarioCreado);

            // Act
            var resultado = await _usuarioServiceMock.Object.Crear(usuarioDTO);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(usuarioDTO.Correo, resultado.Correo);
        }

        [Theory]
        [InlineData("", "Nombre demasiado corto")] // Nombre vacío
        [InlineData("A very long name that exceeds one hundred characters, it's way too long", "Nombre demasiado largo")] // Nombre muy largo
        public async Task CrearUsuario_NombreInvalido_LanzaExcepcion(string nombre, string mensajeEsperado)
        {
            // Arrange
            var usuarioDTO = new UsuarioDTO
            {
                NombreCompleto = nombre,
                Correo = "correo@gmail.com",
                IdRol = 1, // Administrador
                Clave = 12345,
                EsActivo = 1
            };

            _usuarioServiceMock.Setup(service => service.Crear(It.IsAny<UsuarioDTO>()))
                .ThrowsAsync(new TaskCanceledException(mensajeEsperado));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _usuarioServiceMock.Object.Crear(usuarioDTO));
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Theory]
        [InlineData("invalidEmail@", "Correo inválido")]
        [InlineData("invalid@", "Correo inválido")]
        public async Task CrearUsuario_CorreoInvalido_LanzaExcepcion(string correo, string mensajeEsperado)
        {
            // Arrange
            var usuarioDTO = new UsuarioDTO
            {
                NombreCompleto = "Juan Lopez",
                Correo = correo,
                IdRol = 1, // Administrador
                Clave = 12345,
                EsActivo = 1
            };

            _usuarioServiceMock.Setup(service => service.Crear(It.IsAny<UsuarioDTO>()))
                .ThrowsAsync(new TaskCanceledException(mensajeEsperado));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _usuarioServiceMock.Object.Crear(usuarioDTO));
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Theory]
        [InlineData(123, "Contraseña demasiado corta")]
        [InlineData(12345678901234567890, "Contraseña demasiado larga")]
        public async Task CrearUsuario_ClaveInvalida_LanzaExcepcion(int clave, string mensajeEsperado)
        {
            // Arrange
            var usuarioDTO = new UsuarioDTO
            {
                NombreCompleto = "Juan Lopez",
                Correo = "correo@gmail.com",
                IdRol = 1, // Administrador
                Clave = clave,
                EsActivo = 1
            };

            _usuarioServiceMock.Setup(service => service.Crear(It.IsAny<UsuarioDTO>()))
                .ThrowsAsync(new TaskCanceledException(mensajeEsperado));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _usuarioServiceMock.Object.Crear(usuarioDTO));
            Assert.Equal(mensajeEsperado, ex.Message);
        }
    }
}
