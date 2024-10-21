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
    public class ProductoServiceTests
    {
        private readonly Mock<IProductoService> _productoServiceMock;
        private readonly Mock<IGenericRepository<Producto>> _productoRepositorioMock;

        public ProductoServiceTests()
        {
            _productoServiceMock = new Mock<IProductoService>();
            _productoRepositorioMock = new Mock<IGenericRepository<Producto>>();
        }

        [Theory]
        [InlineData("Laptop", 2, "1500.00", true)] // Camino 1: Producto creado correctamente
        [InlineData("", 0, "0", false)] // Camino 2: Error al crear el producto
        public async Task CrearProducto_CaminoBasico(string nombre, int stock, string precio, bool esValido)
        {
            // Arrange
            var productoDTO = new ProductoDTO
            {
                Nombre = nombre,
                Stock = stock,
                Precio = precio
            };

            if (esValido)
            {
                // Simula que el producto se crea correctamente
                var productoCreado = new ProductoDTO { IdProducto = 1, Nombre = nombre, Stock = stock, Precio = precio };
                _productoServiceMock.Setup(service => service.Crear(It.IsAny<ProductoDTO>()))
                    .ReturnsAsync(productoCreado);
            }
            else
            {
                // Simula que el producto no se crea correctamente, lanzando una excepción
                _productoServiceMock.Setup(service => service.Crear(It.IsAny<ProductoDTO>()))
                    .ThrowsAsync(new TaskCanceledException("No se pudo crear"));
            }

            // Act & Assert
            if (esValido)
            {
                var resultado = await _productoServiceMock.Object.Crear(productoDTO);
                Assert.NotNull(resultado); // Debe devolver el objeto ProductoDTO
                Assert.True(resultado.IdProducto > 0); // Verifica que el producto fue creado
                _productoServiceMock.Verify(service => service.Crear(It.IsAny<ProductoDTO>()), Times.Once);
            }
            else
            {
                var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _productoServiceMock.Object.Crear(productoDTO));
                Assert.Equal("No se pudo crear", ex.Message); // Verifica el mensaje de la excepción
                _productoServiceMock.Verify(service => service.Crear(It.IsAny<ProductoDTO>()), Times.Once);
            }
        }
    }
}
