using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents {

    public class MenuViewComponent : ViewComponent {
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;

        public MenuViewComponent(IMenuService menuService, IMapper mapper) {
            _menuService = menuService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            ClaimsPrincipal claimUser = HttpContext.User;

            List<VMMenu> listaMenus = new List<VMMenu> {};

            if (claimUser.Identity.IsAuthenticated) {
                string idUsuario = claimUser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                List<Menu> lista = await _menuService.ObtenerMenus(int.Parse(idUsuario));
                listaMenus = _mapper.Map<List<VMMenu>>(lista);
            }

            return View(listaMenus);
        }
    }
}
