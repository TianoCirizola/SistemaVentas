using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.BLL.Implementacion;

namespace SistemaVenta.AplicacionWeb.Controllers {

    public class CategoriaController : Controller {

        private readonly IMapper _mapper;
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(IMapper mapper, ICategoriaService categoriaService) {
            _mapper = mapper;
            _categoriaService = categoriaService;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaCategorias() {
            List<Categoria> lista = await _categoriaService.Lista();
            List<VMCategoria> vmCategoriaLista = _mapper.Map<List<VMCategoria>>(lista);
            
            return StatusCode(StatusCodes.Status200OK, new { data =  vmCategoriaLista });
        }

        [HttpPost]
        public async Task<IActionResult> CrearCategoria([FromBody] VMCategoria modelo) {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try {
                Categoria categoriaCreada = await _categoriaService.Crear(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VMCategoria>(categoriaCreada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex) {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> EditarCategoria([FromBody] VMCategoria modelo) {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try {
                Categoria categoriaEditada = await _categoriaService.Editar(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VMCategoria>(categoriaEditada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex) {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> EliminarCategoria(int idCategoria) {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try {
                gResponse.Estado = await _categoriaService.Eliminar(idCategoria);
            }
            catch (Exception ex) {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}