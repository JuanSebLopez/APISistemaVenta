using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVenta.BLL.Services.Contracts;
using SistemaVenta.DAL.Repositories.Contracts;
using SistemaVenta.DTO;
using SistemaVenta.Model;

namespace SistemaVenta.BLL.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _categoriaRepositorio;
        private readonly IMapper _mapper;

        public CategoriaService(IGenericRepository<Categoria> categoriaRepositorio, IMapper mapper)
        {
            _categoriaRepositorio = categoriaRepositorio;
            _mapper = mapper;
        }
        public async Task<List<CategoriaDTO>> Lista()
        {
            try
            {
                var listaCategorias = await _categoriaRepositorio.Consult();

                return _mapper.Map<List<CategoriaDTO>>(listaCategorias.ToList());
            } 
            catch
            {
                throw;
            }
        }

        public async Task<CategoriaDTO> Crear(CategoriaDTO modelo)
        {
            try
            {
                var categoriaCreada = await _categoriaRepositorio.Create(_mapper.Map<Categoria>(modelo));

                if (categoriaCreada.IdCategoria == 0)
                    throw new TaskCanceledException("No se pudo crear");

                return _mapper.Map<CategoriaDTO>(categoriaCreada);
            }
            catch
            { 
                throw; 
            }
        }

        public async Task<bool> Editar(CategoriaDTO modelo)
        {
            try
            {
                var categoriaModelo = _mapper.Map<Categoria>(modelo);

                var categoriaEncontrada = await _categoriaRepositorio.Get(u =>
                    u.IdCategoria == categoriaModelo.IdCategoria);

                if (categoriaEncontrada == null)
                    throw new TaskCanceledException("La categoria no existe");

                categoriaEncontrada.Nombre = categoriaModelo.Nombre;
                categoriaEncontrada.EsActivo = categoriaModelo.EsActivo;

                bool respuesta = await _categoriaRepositorio.Edit(categoriaEncontrada);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar");

                return respuesta;
            }
            catch
            { 
                throw; 
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var categoriaEncontrada = await _categoriaRepositorio.Get(u =>u.IdCategoria == id);

                if (categoriaEncontrada == null)
                    throw new TaskCanceledException("La categoria no existe");

                bool respuesta = await _categoriaRepositorio.Delete(categoriaEncontrada);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo eliminar");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }
    }
}
