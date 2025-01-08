using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFireBaseService _firebaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;

        public UsuarioService (
            IGenericRepository<Usuario> repositorio,
            IFireBaseService firebaseService,
            IUtilidadesService utilidadesService,
            ICorreoService correoService
            )
        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }


        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Usuario existeUsuario = await _repositorio.Obtener(u => u.Correo == entidad.Correo);

            if (existeUsuario == null) throw new TaskCanceledException("El correo ya existe");

            try
            {
                string claveGenerada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(claveGenerada);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto = urlFoto;
                }

                Usuario usuarioCreado = await _repositorio.Crear(entidad);
                if (usuarioCreado.IdUsuario == 0) throw new TaskCanceledException("El correo ya existe");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]", claveGenerada);

                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                            {
                                readerStream = new StreamReader(dataStream);
                            }
                            else
                            {
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            }

                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }
                    if (htmlCorreo != "") await _correoService.EnviarCorreo(usuarioCreado.Correo, "Cuenta Creada", htmlCorreo);
                }
                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(r => r.IdRolNavigation).First();

                return usuarioCreado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int IdUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GuardarPerfil(Usuario entidad)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.Include(r => r.IdRolNavigation).ToList();
        }

        public Task<Usuario> ObtenerPorCredenciales(string Correo, string Clave)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> ObtenerPorId(int IdUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestablecerClave(string Correo, string UrlPlantilla)
        {
            throw new NotImplementedException();
        }
    }
}
