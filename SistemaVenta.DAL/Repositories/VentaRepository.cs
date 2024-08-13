using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositories.Contracts;
using SistemaVenta.Model;
    
namespace SistemaVenta.DAL.Repositories
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dbcontext;

        public VentaRepository(DbventaContext dbcontext) : base(dbcontext) 
        {
            _dbcontext = dbcontext;
        }

        public async Task<Venta> Register(Venta model)
        {
            Venta ventaGenerada = new Venta();

            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in model.DetalleVenta) {
                        Producto producto_encontrado = _dbcontext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();

                        producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad;
                        _dbcontext.Productos.Update(producto_encontrado);
                    }
                    await _dbcontext.SaveChangesAsync();

                    NumeroDocumento correlative = _dbcontext.NumeroDocumentos.First();

                    correlative.UltimoNumero = correlative.UltimoNumero + 1;
                    correlative.FechaRegistro = DateTime.Now;

                    _dbcontext.NumeroDocumentos.Update(correlative);
                    await _dbcontext.SaveChangesAsync();

                    int cantidadDigitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", cantidadDigitos));
                    string numeroVenta = ceros + correlative.UltimoNumero.ToString();

                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - cantidadDigitos, cantidadDigitos);

                    model.NumeroDocumento = numeroVenta;

                    await _dbcontext.Venta.AddAsync(model);
                    await _dbcontext.SaveChangesAsync();

                    ventaGenerada = model;

                    transaction.Commit();
                }
                catch { 
                    transaction.Rollback();
                    throw;
                }

                return ventaGenerada;
            }
        }
    }
}
