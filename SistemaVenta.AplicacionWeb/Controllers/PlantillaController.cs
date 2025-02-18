using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.Interfaces;
using Microsoft.AspNetCore.Components.Forms.Mapping;

namespace SistemaVenta.AplicacionWeb.Controllers {

    public class PlantillaController : Controller {

        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;
        private readonly IVentaService _ventaService;

        public PlantillaController(IMapper mapper, INegocioService negocioService, IVentaService ventaService) {
            _mapper = mapper;
            _negocioService = negocioService;
            _ventaService = ventaService;
        }

        public IActionResult EnviarClave(string correo, string clave) {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";

            return View();
        }

        public IActionResult RestablecerClave(string clave) {
            ViewData["Clave"] = clave;

            return View();
        }

        public async Task <IActionResult> PDFVenta(string numeroVenta) {
            VMVenta vmVenta = _mapper.Map<VMVenta>(await _ventaService.Detalle(numeroVenta));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

            VMPDFVenta modelo = new VMPDFVenta();

            modelo.Negocio = vmNegocio;
            modelo.Venta = vmVenta;

            return View(modelo);
        }
    }
}
