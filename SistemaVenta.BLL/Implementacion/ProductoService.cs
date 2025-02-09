using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion {

    public class ProductoService : IProductoService {

        private readonly IGenericRepository<Producto> _repositorio;
        private readonly IFireBaseService _firebaseService;

        public ProductoService(IGenericRepository<Producto> repositorio, IFireBaseService firebaseService) {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
        }

        public async Task<List<Producto>> Lista() {
            IQueryable<Producto> query = await _repositorio.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }

        public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "") {
            Producto productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra);

            if (productoExiste != null) throw new TaskCanceledException("El código de barra ya existe");

            try {
                entidad.NombreImagen = nombreImagen;
                if (imagen != null) {
                    string urlImagen = await _firebaseService.SubirStorage(imagen, "carpeta_producto", nombreImagen);
                    entidad.UrlImagen = urlImagen;
                }

                Producto productoCreado = await _repositorio.Crear(entidad);

                if (productoCreado.IdProducto == 0) throw new TaskCanceledException("No se pudo crear el producto");

                IQueryable<Producto> query = await _repositorio.Consultar(p => p.IdProducto == productoCreado.IdProducto);
                productoCreado = query.Include(c => c.IdCategoriaNavigation).First();

                return productoCreado;
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<Producto> Editar(Producto entidad, Stream imagen = null, string nombreImagen = "") {
            Producto productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto);

            if (productoExiste != null) throw new TaskCanceledException("El código de barra ya existe");

            try {
                IQueryable<Producto> query = await _repositorio.Consultar(p => p.IdProducto == entidad.IdProducto);
                Producto productoEditar = query.First();

                productoEditar.CodigoBarra = entidad.CodigoBarra;
                productoEditar.Marca = entidad.Marca;
                productoEditar.Descripcion = entidad.Descripcion;
                productoEditar.IdCategoria = entidad.IdCategoria;
                productoEditar.Stock = entidad.Stock;
                productoEditar.Precio = entidad.Precio;
                productoEditar.EsActivo = entidad.EsActivo;

                if (productoEditar.NombreImagen == "") productoEditar.NombreImagen = nombreImagen;

                if (imagen != null) {
                    string urlImagen = await _firebaseService.SubirStorage(imagen, "carpeta_producto", productoEditar.NombreImagen);
                    productoEditar.UrlImagen = urlImagen;
                }

                bool respuesta = await _repositorio.Editar(productoEditar);
                if (!respuesta) throw new TaskCanceledException("No se pudo modificar el producto");

                Producto productoEditado = query.Include(c => c.IdCategoriaNavigation).First();

                return productoEditado;
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idProducto) {
            try {
                Producto productoEncontrado = await _repositorio.Obtener(p => p.IdProducto == idProducto);

                if (productoEncontrado == null) throw new TaskCanceledException("El producto no existe");

                string nombreImagen = productoEncontrado.NombreImagen;

                bool respuesta = await _repositorio.Eliminar(productoEncontrado);

                if (respuesta) await _firebaseService.EliminarStorage("carpeta_producto", nombreImagen);

                return true;
            }
            catch {
                throw;
            }
        }
    }
}