using AutoMapper;
using Moq;
using SistemaVenta.BLL.Services;
using SistemaVenta.DAL.Repositories.Contracts;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using SistemaVenta.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Moq.EntityFrameworkCore;

namespace SistemaVenta.Tests.Pruebas_Unitarias.Camino_Básico
{
    public class VentaServiceTest
    {
        private readonly Mock<IVentaRepository> _ventaRepositoryMock;
        private readonly IMapper _mapper;
        private readonly VentaService _ventaService;

        public VentaServiceTest()
        {
            // Inicializamos el mock del repositorio de ventas
            _ventaRepositoryMock = new Mock<IVentaRepository>();

            // Configuración de AutoMapper utilizando Profile de AutoMapper
            var mockMapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = mockMapperConfig.CreateMapper();

            // Inicializamos el servicio de ventas con las dependencias mockeadas
            _ventaService = new VentaService(_ventaRepositoryMock.Object, null, _mapper);
        }

        [Fact]
        public async Task Record_BuscarPorFecha_RetornaListaDeVentas()
        {
            // Arrange: Preparar el escenario del test
            var ventas = new List<Venta>
        {
            new Venta
            {
                IdVenta = 1,
                NumeroDocumento = "001",
                FechaRegistro = new DateTime(2024, 01, 15),
                Total = 1000,
                DetalleVenta = new List<DetalleVenta>()
            },
            new Venta
            {
                IdVenta = 2,
                NumeroDocumento = "002",
                FechaRegistro = new DateTime(2024, 01, 20),
                Total = 2000,
                DetalleVenta = new List<DetalleVenta>()
            }
        }.AsQueryable();

            // Configuramos el mock del repositorio para devolver las ventas simuladas
            _ventaRepositoryMock.Setup(repo => repo.Consult(null))
                .ReturnsAsync(new TestAsyncEnumerable<Venta>(ventas)); // Usamos TestAsyncEnumerable

            // Act: Llamamos al método bajo prueba
            var result = await _ventaService.Record("fecha", null, "01/01/2024", "31/01/2024");

            // Assert: Verificamos que el resultado sea el esperado
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Verificamos que devuelve 2 ventas
            Assert.Equal("001", result[0].NumeroDocumento);
            Assert.Equal("002", result[1].NumeroDocumento);
        }

        [Fact]
        public async Task Record_BuscarPorNumeroVenta_RetornaVentaCorrespondiente()
        {
            // Arrange: Preparar el escenario del test
            var ventas = new List<Venta>
        {
            new Venta
            {
                IdVenta = 1,
                NumeroDocumento = "001",
                FechaRegistro = new DateTime(2024, 01, 15),
                Total = 1000,
                DetalleVenta = new List<DetalleVenta>()
            }
        }.AsQueryable();

            // Configuramos el mock del repositorio para devolver una venta simulada por número de documento
            _ventaRepositoryMock.Setup(repo => repo.Consult(null))
                .ReturnsAsync(new TestAsyncEnumerable<Venta>(ventas)); // Usamos TestAsyncEnumerable

            // Act: Llamamos al método bajo prueba
            var result = await _ventaService.Record("numeroVenta", "001", null, null);

            // Assert: Verificamos que el resultado sea el esperado
            Assert.NotNull(result);
            Assert.Single(result); // Verificamos que devuelve una sola venta
            Assert.Equal("001", result[0].NumeroDocumento);
        }
    }
}
