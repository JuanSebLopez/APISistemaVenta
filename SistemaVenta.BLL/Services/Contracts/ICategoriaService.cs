using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DTO;

namespace SistemaVenta.BLL.Services.Contracts
{
    public interface ICategoriaService
    {
        Task<List<CategoriaDTO>> Lista();
        Task<CategoriaDTO> Crear(CategoriaDTO categoriaDTO);
        Task<bool> Editar (CategoriaDTO categoriaDTO);
        Task<bool> Eliminar(int id);
    }
}
