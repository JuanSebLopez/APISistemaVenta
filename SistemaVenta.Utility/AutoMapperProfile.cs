using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVenta.DTO;
using SistemaVenta.Model;

namespace SistemaVenta.Utility
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Rol
            CreateMap<Rol, RolDTO>().ReverseMap();
            #endregion Rol

            #region Menu
            CreateMap<Menu, MenuDTO>().ReverseMap();
            #endregion Menu

            #region Usuario
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(route => 
                    route.RolDescripcion, 
                    opt => opt.MapFrom(origin => origin.IdRolNavigation.Nombre)
                )
                .ForMember(route => 
                    route.EsActivo,
                    opt => opt.MapFrom(origin => origin.EsActivo == true ? 1 : 0)
                );

            CreateMap<Usuario, SesionDTO>()
                .ForMember(route =>
                    route.RolDescripcion,
                    opt => opt.MapFrom(origin => origin.IdRolNavigation.Nombre)
                );

            CreateMap<UsuarioDTO, Usuario>()
                .ForMember(route =>
                    route.IdRolNavigation,
                    opt => opt.Ignore()
                )
                .ForMember(route =>
                    route.EsActivo,
                    opt => opt.MapFrom(origin => origin.EsActivo == 1 ? true : false)
                );
            #endregion Usuario

            #region Categoria
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
            #endregion Categoria

            #region Producto
            CreateMap<Producto, ProductoDTO>()
                .ForMember(route =>
                    route.CategoriaDescripcion,
                    opt => opt.MapFrom(origin => origin.IdCategoriaNavigation.Nombre)
                )
                .ForMember(route => 
                    route.Precio,
                    opt => opt.MapFrom(origin => Convert.ToString(origin.Precio.Value, new CultureInfo("es-CO")))
                )
                .ForMember(route =>
                    route.EsActivo,
                    opt => opt.MapFrom(origin => origin.EsActivo == true ? 1 : 0)
                );
            
            CreateMap<ProductoDTO, Producto>()
                .ForMember(route =>
                    route.IdCategoriaNavigation,
                    opt => opt.Ignore()
                )
                .ForMember(route =>
                    route.Precio,
                    opt => opt.MapFrom(origin => Convert.ToDecimal(origin.Precio, new CultureInfo("es-CO")))
                )
                .ForMember(route =>
                    route.EsActivo,
                    opt => opt.MapFrom(origin => origin.EsActivo == 1 ? true : false)
                );
            #endregion Producto

            #region Venta
            CreateMap<Venta, VentaDTO>()
                .ForMember(route =>
                    route.TotalTexto,
                    opt => opt.MapFrom(origin => Convert.ToString(origin.Total.Value, new CultureInfo("es-CO")))
                )
                .ForMember(route =>
                    route.FechaRegistro,
                    opt => opt.MapFrom(origin => origin.FechaRegistro.Value.ToString("dd/MM/yyyy"))
                );

            CreateMap<VentaDTO, Venta>()
                .ForMember(route =>
                    route.Total,
                    opt => opt.MapFrom(origin => Convert.ToDecimal(origin.TotalTexto, new CultureInfo("es-CO")))
                );
            #endregion Venta

            #region DetalleVenta
            CreateMap<DetalleVenta, DetalleVentaDTO>()
                .ForMember(route => 
                    route.ProductoDescripcion,
                    opt => opt.MapFrom(origin => origin.IdProductoNavigation.Nombre)
                )
                .ForMember(route =>
                    route.PrecioTexto,
                    opt => opt.MapFrom(origin => Convert.ToString(origin.Precio.Value, new CultureInfo("es-CO")))
                )
                .ForMember(route =>
                    route.TotalTexto,
                    opt => opt.MapFrom(origin => Convert.ToString(origin.Total.Value, new CultureInfo("es-CO")))
                );

            CreateMap<DetalleVentaDTO, DetalleVenta>()
                .ForMember(route =>
                    route.Precio,
                    opt => opt.MapFrom(origin => Convert.ToDecimal(origin.PrecioTexto, new CultureInfo("es-CO")))
                )
                .ForMember(route =>
                    route.Total,
                    opt => opt.MapFrom(origin => Convert.ToDecimal(origin.TotalTexto, new CultureInfo("es-CO")))
                );
            #endregion DetalleVenta

            #region Reporte
            CreateMap<DetalleVenta, ReporteDTO>()
                .ForMember(route =>
                    route.FechaRegistro,
                    opt => opt.MapFrom(origin => origin.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy"))
                )
                .ForMember(route =>
                    route.NumeroDocumento,
                    opt => opt.MapFrom(origin => origin.IdVentaNavigation.NumeroDocumento)
                )
                .ForMember(route =>
                        route.TipoPago,
                        opt => opt.MapFrom(origin => origin.IdVentaNavigation.TipoPago)
                )
                .ForMember(route =>
                    route.TotalVenta,
                    opt => opt.MapFrom(origin => Convert.ToString(origin.IdVentaNavigation.Total.Value, new CultureInfo("es-CO")))
                )
                .ForMember(route =>
                    route.Producto,
                    opt => opt.MapFrom(origin => origin.IdProductoNavigation.Nombre)
                )
                .ForMember(route =>
                    route.Precio,
                    opt => opt.MapFrom(origin => Convert.ToString(origin.Precio.Value, new CultureInfo("es-CO")))
                )
                .ForMember(route =>
                    route.Total,
                    opt => opt.MapFrom(origin => Convert.ToString(origin.Total.Value, new CultureInfo("es-CO")))
                );
            #endregion Reporte
        }
    }
}
