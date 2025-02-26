using Microsoft.AspNetCore.Mvc;

using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using NuGet.Packaging.Signing;

namespace SistemaVenta.AplicacionWeb.Controllers {

    public class AccesoController : Controller {

        private readonly IUsuarioService _usuarioService;

        public AccesoController(IUsuarioService usuarioService) {
            _usuarioService = usuarioService;
        }

        public IActionResult Login() {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult RestablecerClave() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin modelo) {
            Usuario usuarioEncontrado = await _usuarioService.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);

            if (usuarioEncontrado == null) {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }
            ViewData["Mensaje"] = null;

            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, usuarioEncontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuarioEncontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuarioEncontrado.IdRol.ToString()),
                new Claim("UrlFoto", usuarioEncontrado.UrlFoto),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties() {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesion
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), properties
            );

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMUsuarioLogin modelo) {
            try {
                string urlPlantilla = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";

                bool resultado = await _usuarioService.RestablecerClave(modelo.Correo, urlPlantilla);

                if (resultado) {
                    ViewData["Mensaje"] = "Listo, su contraseña fue restablecida. Revise su correo.";
                    ViewData["MensajeError"] = null;

                } else {
                    ViewData["Mensaje"] = null;
                    ViewData["MensajeError"] = "Ha ocurrido un problema, por favor intente más tarde.";
                }

            }
            catch (Exception ex) {
                ViewData["Mensaje"] = null;
                ViewData["MensajeError"] = ex.Message;
            }

            return View();
        }
    }
}