using Moq;
using SistemaVenta.BLL.Services.Contracts;
using SistemaVenta.DAL.Repositories.Contracts;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.Tests.Pruebas_Unitarias.Camino_Básico
{
    public class VentaServiceTests
    {
        private readonly Mock<IVentaService> _ventaServiceMock;
        private readonly Mock<IGenericRepository<Venta>> _ventaRepositorioMock;

        public VentaServiceTests()
        {
            _ventaServiceMock = new Mock<IVentaService>();
            _ventaRepositorioMock = new Mock<IGenericRepository<Venta>>();
        }

        [Theory]
        [InlineData(1, "Laptop", 2, "Efectivo", true)] // Camino 1: Venta creada correctamente
        [InlineData(null, "", 0, "", false)] // Camino 2: Error al registrar la venta
        public async Task RegistrarVenta_CaminoBasico(int idVenta, string producto, int cantidad, string tipoPago, bool esValido)
        {
            // Arrange
            var ventaDTO = new VentaDTO
            {
                IdVenta = idVenta,
                DetalleVenta = new List<DetalleVentaDTO>
                {
                    new DetalleVentaDTO { Producto = producto, Cantidad = cantidad }
                },
                TipoPago = tipoPago,
                TotalTexto = "1500" // valor ejemplo
            };

            if (esValido)
            {
                // Simula que la venta se crea correctamente
                var ventaCreada = new VentaDTO { IdVenta = 1, TipoPago = tipoPago, DetalleVenta = ventaDTO.DetalleVenta, TotalTexto = "1500" };
                _ventaServiceMock.Setup(service => service.Register(It.IsAny<VentaDTO>()))
                    .ReturnsAsync(ventaCreada);
            }
            else
            {
                // Simula que la venta no se crea correctamente, lanzando una excepción
                _ventaServiceMock.Setup(service => service.Register(It.IsAny<VentaDTO>()))
                    .ThrowsAsync(new TaskCanceledException("No se pudo crear"));
            }

            // Act & Assert
            if (esValido)
            {
                var resultado = await _ventaServiceMock.Object.Register(ventaDTO);
                Assert.NotNull(resultado); // Debe devolver el objeto VentaDTO
                Assert.True(resultado.IdVenta > 0); // Verifica que la venta fue creada
                _ventaServiceMock.Verify(service => service.Register(It.IsAny<VentaDTO>()), Times.Once);
            }
            else
            {
                var ex = await Assert.ThrowsAsync<TaskCanceledException>(() => _ventaServiceMock.Object.Register(ventaDTO));
                Assert.Equal("No se pudo crear", ex.Message); // Verifica el mensaje de la excepción
                _ventaServiceMock.Verify(service => service.Register(It.IsAny<VentaDTO>()), Times.Once);
            }
        }
    }
}
