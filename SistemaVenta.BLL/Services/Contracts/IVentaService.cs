using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DTO;

namespace SistemaVenta.BLL.Services.Contracts
{
    public interface IVentaService
    {
        Task<VentaDTO> Regsitrar(VentaDTO modelo);
        Task<VentaDTO> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin);
    }
}
