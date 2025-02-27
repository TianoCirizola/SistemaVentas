using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers {

    [Authorize]

    public class ReporteController : Controller {

        private readonly IMapper _mapper;
        private readonly IVentaService _ventaService;

        public ReporteController(IMapper mapper, IVentaService ventaService) {
            _mapper = mapper;
            _ventaService = ventaService;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReporteVenta(string fechaInicio, string fechaFin) {
            List<DetalleVenta> lista = await _ventaService.ReporteVenta(fechaInicio, fechaFin);
            List<VMReporteVenta> vmLista =  _mapper.Map<List<VMReporteVenta>>(lista);

            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }
    }
}