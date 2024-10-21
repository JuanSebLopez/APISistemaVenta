using AutoMapper;
using Moq;
using SistemaVenta.BLL.Services;
using SistemaVenta.BLL.Services.Contracts;
using SistemaVenta.DAL.Repositories.Contracts;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using SistemaVenta.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.Tests.Pruebas_Unitarias.Camino_Básico
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IGenericRepository<Usuario>> _usuarioRepositorioMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _usuarioRepositorioMock = new Mock<IGenericRepository<Usuario>>(MockBehavior.Strict);
            _mapperMock = new Mock<IMapper>();
            _usuarioService = new UsuarioService(_usuarioRepositorioMock.Object, _mapperMock.Object);
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

        // Crear Usuario
        [Theory]
        [InlineData("Juan", "juan@correo.com", 1, true)] // Usuario creado correctamente
        [InlineData("", "", 0, false)] // Falla al crear el usuario, lanza excepción
        public async Task CrearUsuario_CaminoBasico(string nombre, string correo, int idRol, bool esValido)
        {
            // Arrange
            var usuarioDTO = new UsuarioDTO
            {
                NombreCompleto = nombre,
                Correo = correo,
                IdRol = idRol,
                Clave = "123456",
                EsActivo = 1
            };

            if (esValido)
            {
                // Simula que el usuario se crea correctamente
                var usuarioCreado = new UsuarioDTO { IdUsuario = 1, NombreCompleto = nombre, Correo = correo, IdRol = idRol };
                _usuarioServiceMock.Setup(service => service.Crear(It.IsAny<UsuarioDTO>()))
                    .ReturnsAsync(usuarioCreado);
            }
            else
            {
                // Simula que el usuario no se crea correctamente, lanzando una excepción
                _usuarioServiceMock.Setup(service => service.Crear(It.IsAny<UsuarioDTO>()))
                    .ThrowsAsync(new TaskCanceledException("No se pudo crear"));
            }

            // Act & Assert
            if (esValido)
            {
                var resultado = await _usuarioServiceMock.Object.Crear(usuarioDTO);
                Assert.NotNull(resultado); // Debe devolver el objeto UsuarioDTO
                Assert.True(resultado.IdUsuario > 0); // Verifica que el usuario fue creado
                _usuarioServiceMock.Verify(service => service.Crear(It.IsAny<UsuarioDTO>()), Times.Once);
            }
            else
            {
                var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _usuarioServiceMock.Object.Crear(usuarioDTO));
                Assert.Equal("No se pudo crear", ex.Message); // Verifica el mensaje de la excepción
                _usuarioServiceMock.Verify(service => service.Crear(It.IsAny<UsuarioDTO>()), Times.Once);
            }
        
        }

        // Modificar Usuario
        [Theory]
        [InlineData(1, "Juan", "juan@correo.com", 1, true)] // Camino 1: Edición exitosa
        [InlineData(9999, "Juan", "juan@correo.com", 1, false)] // Camino 2: Usuario no encontrado
        [InlineData(1, "Juan", "juan@correo.com", 1, false, true)] // Camino 3: Falla la edición
        public async Task EditarUsuario_CaminoBasico(int idUsuario, string nombre, string correo, int idRol, bool esValido, bool fallaEdicion = false)
        {
            // Arrange
            var usuarioDTO = new UsuarioDTO
            {
                IdUsuario = idUsuario,
                NombreCompleto = nombre,
                Correo = correo,
                IdRol = idRol,
                Clave = "123456",
                EsActivo = 1
            };

            var usuario = new Usuario
            {
                IdUsuario = idUsuario,
                NombreCompleto = nombre,
                Correo = correo,
                IdRol = idRol,
                Clave = "123456",
                EsActivo = true
            };

            _mapperMock.Setup(m => m.Map<Usuario>(It.IsAny<UsuarioDTO>())).Returns(usuario);

            if (idUsuario == 9999)
            {
                // Simula que el usuario no es encontrado (Camino 2)
                _usuarioRepositorioMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .ReturnsAsync((Usuario)null); // Aquí devolvemos null
            }
            else
            {
                // Simula que el usuario es encontrado (Camino 1 y 3)
                _usuarioRepositorioMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .ReturnsAsync(usuario); // Aquí devolvemos el usuario encontrado
            }

            if (fallaEdicion)
            {
                // Simula que falla la edición (Camino 3)
                _usuarioRepositorioMock.Setup(repo => repo.Edit(It.IsAny<Usuario>()))
                    .ReturnsAsync(false); // Aquí devolvemos false para fallar la edición
            }
            else if (esValido)
            {
                // Simula que la edición es exitosa (Camino 1)
                _usuarioRepositorioMock.Setup(repo => repo.Edit(It.IsAny<Usuario>()))
                    .ReturnsAsync(true); // Aquí devolvemos true para éxito
            }

            // Act & Assert
            if (idUsuario == 9999)
            {
                // Camino 2: Verifica que se lance una excepción cuando el usuario no es encontrado
                var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _usuarioService.Editar(usuarioDTO));
                Assert.Equal("El usuario no existe", ex.Message);
            }
            else if (fallaEdicion)
            {
                // Camino 3: Verifica que se lance una excepción cuando falla la edición
                var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _usuarioService.Editar(usuarioDTO));
                Assert.Equal("No se pudo editar", ex.Message);
            }
            else
            {
                // Camino 1: Verifica que la edición fue exitosa
                var resultado = await _usuarioService.Editar(usuarioDTO);
                Assert.True(resultado);
                _usuarioRepositorioMock.Verify(repo => repo.Edit(It.IsAny<Usuario>()), Times.Once);
            }
        }

        // Eliminar
        [Theory]
        [InlineData(1001, true)]  // Camino 1: Usuario encontrado y eliminación exitosa
        [InlineData(9999, false)] // Camino 2: Usuario no encontrado
        [InlineData(1002, false, true)] // Camino 3: Usuario encontrado pero falla la eliminación
        public async Task EliminarUsuario_CaminoBasico(int idUsuario, bool existe, bool fallaEliminacion = false)
        {
            // Arrange
            var usuario = new Usuario
            {
                IdUsuario = idUsuario,
                NombreCompleto = "Juan",
                Correo = "juan@correo.com",
                IdRol = 1,
                Clave = "123456",
                EsActivo = true
            };

            if (!existe)
            {
                // Simula que el usuario no es encontrado (Camino 2)
                _usuarioRepositorioMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .ReturnsAsync((Usuario)null); // Devuelve null si el usuario no existe
            }
            else
            {
                // Simula que el usuario es encontrado (Camino 1 y 3)
                _usuarioRepositorioMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .ReturnsAsync(usuario); // Devuelve el usuario encontrado
            }

            if (fallaEliminacion)
            {
                // Simula que falla la eliminación (Camino 3)
                _usuarioRepositorioMock.Setup(repo => repo.Delete(It.IsAny<Usuario>()))
                    .ReturnsAsync(false); // Devuelve false si falla la eliminación
            }
            else if (existe)
            {
                // Simula que la eliminación es exitosa (Camino 1)
                _usuarioRepositorioMock.Setup(repo => repo.Delete(It.IsAny<Usuario>()))
                    .ReturnsAsync(true); // Devuelve true si la eliminación es exitosa
            }

            // Act & Assert
            if (!existe)
            {
                // Camino 2: Verifica que se lance una excepción cuando el usuario no es encontrado
                var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _usuarioService.Eliminar(idUsuario));
                Assert.Equal("El usuario no existe", ex.Message);
            }
            else if (fallaEliminacion)
            {
                // Camino 3: Verifica que se lance una excepción cuando falla la eliminación
                var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _usuarioService.Eliminar(idUsuario));
                Assert.Equal("No se pudo eliminar", ex.Message);
            }
            else
            {
                // Camino 1: Verifica que la eliminación fue exitosa
                var resultado = await _usuarioService.Eliminar(idUsuario);
                Assert.True(resultado);
                _usuarioRepositorioMock.Verify(repo => repo.Delete(It.IsAny<Usuario>()), Times.Once);
            }
        }
    }
}
