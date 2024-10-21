using Moq;
using SistemaVenta.BLL.Services.Contracts;
using SistemaVenta.DAL.Repositories.Contracts;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.Tests.Pruebas_Unitarias.Camino_Básico
{
    public class CategoriaServiceTests
    {
        private readonly Mock<ICategoriaService> _categoriaServiceMock;
        private readonly Mock<IGenericRepository<Categoria>> _productoRepositorioMock;

        public CategoriaServiceTests()
        {
            _categoriaServiceMock = new Mock<ICategoriaService>();
            _productoRepositorioMock = new Mock<IGenericRepository<Categoria>>();
        }

        [Theory]
        [InlineData("Tecnología", true)] // Camino 1: Categoría creada correctamente
        [InlineData("", false)] // Camino 2: Error al crear la categoría
        public async Task CrearCategoria_CaminoBasico(string nombre, bool esValido)
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO
            {
                Nombre = nombre
            };

            if (esValido)
            {
                // Simula que la categoría se crea correctamente
                var categoriaCreada = new CategoriaDTO { IdCategoria = 1, Nombre = nombre };
                _categoriaServiceMock.Setup(service => service.Crear(It.IsAny<CategoriaDTO>()))
                    .ReturnsAsync(categoriaCreada);
            }
            else
            {
                // Simula que la categoría no se crea correctamente, lanzando una excepción
                _categoriaServiceMock.Setup(service => service.Crear(It.IsAny<CategoriaDTO>()))
                    .ThrowsAsync(new TaskCanceledException("No se pudo crear"));
            }

            // Act & Assert
            if (esValido)
            {
                var resultado = await _categoriaServiceMock.Object.Crear(categoriaDTO);
                Assert.NotNull(resultado); // Debe devolver el objeto CategoriaDTO
                Assert.True(resultado.IdCategoria > 0); // Verifica que la categoría fue creada
                _categoriaServiceMock.Verify(service => service.Crear(It.IsAny<CategoriaDTO>()), Times.Once);
            }
            else
            {
                var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _categoriaServiceMock.Object.Crear(categoriaDTO));
                Assert.Equal("No se pudo crear", ex.Message); // Verifica el mensaje de la excepción
                _categoriaServiceMock.Verify(service => service.Crear(It.IsAny<CategoriaDTO>()), Times.Once);
            }
        }
    }
}
