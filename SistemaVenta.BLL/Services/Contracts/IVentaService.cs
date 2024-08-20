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
        Task<VentaDTO> Register(VentaDTO modelo);
        Task <List<VentaDTO>> Record(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin);
        Task <List<ReporteDTO>> Report(string fechaInicio, string fechaFin);
    }
}
