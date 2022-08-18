using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UforyAPIREST.DataService.Interface;
using UforyAPIREST.Handlers;
using UforyAPIREST.Models;
using UforyAPIREST.Models.Requests.DibujosRequest;

namespace UforyAPIREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DibujosController : Controller
    {
        private readonly IArchivosService _archivosService;
        private readonly IUsuariosService _usuariosService;
        private readonly IDibujosService _dibujosService;
        private JWTHandler tokenHandler;

        public DibujosController(IArchivosService archivosService, IUsuariosService usuariosService, IDibujosService dibujosService)
        {
            _archivosService = archivosService;
            _usuariosService = usuariosService;
            _dibujosService = dibujosService;
        }

        [HttpGet("{idArchivo}")]
        public IActionResult Get(int idArchivo)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El archivo existe

            El archivo pertenece al usuario
            */

            tokenHandler = new JWTHandler((ClaimsIdentity)User.Identity);

            var claimIdUsuario = tokenHandler.GetId(); //"user_id" del tokenHandler
            var claimSesion = tokenHandler.GetSessionId(); //"session_id" del tokenHandler

            //Traigo al usuario de la DB
            var usuarioDB = _usuariosService.GetUsuario(claimIdUsuario);

            //Comprueba que el usuario exista
            if (!UsuariosHandler.Existe(usuarioDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Traigo al archivo de la DB
            var archivoDB = _archivosService.GetArchivo(idArchivo);

            //Compruebo que el archivo exista
            if (!ArchivosHandler.Existe(archivoDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Compruebo que el archivo pertenezca al usuario
            if (!(archivoDB.IdUsuario == claimIdUsuario))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            var dibujosDB = _dibujosService.GetDibujosEnArchivo(idArchivo);

            switch (archivoDB.Tipo)
            {
                case "img":
                    return Ok(Converter.LDibujoALDibujosImagenResponse(dibujosDB));
                case "aud":
                    return Ok(Converter.LDibujoALDibujosAudioResponse(dibujosDB));
                case "vid":
                    return Ok(Converter.LDibujoALDibujosVideoResponse(dibujosDB));
                default:
                    return Ok(new { msgOk = "No fue posible realizar la acción." });
            }
        }


        [HttpPost("upload")]
        public IActionResult PostDibujoAudio(DibujosInsert dibujoSubir)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El archivo existe

            El archivo pertenece al usuario

            El identificador es valido

            El identificador no pertenece a ningun dibujo

            La duración es válida (Si es distinto de null)
            */

            tokenHandler = new JWTHandler((ClaimsIdentity)User.Identity);

            var claimIdUsuario = tokenHandler.GetId(); //"user_id" del tokenHandler
            var claimSesion = tokenHandler.GetSessionId(); //"session_id" del tokenHandler



            //Traigo al usuario de la DB
            var usuarioDB = _usuariosService.GetUsuario(claimIdUsuario);

            //Comprueba que el usuario exista
            if (!UsuariosHandler.Existe(usuarioDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Traigo al archivo de la DB
            var archivoDB = _archivosService.GetArchivo(dibujoSubir.IdArchivo);

            //Compruebo que el archivo exista
            if (!ArchivosHandler.Existe(archivoDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Compruebo que el archivo pertenezca al usuario
            if (!(archivoDB.IdUsuario == claimIdUsuario))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Compruebo que el identificador sea un guid
            if (!DibujosHandler.IdentificadorValido(dibujoSubir.Identificador))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que no exista un comentario con ese identificador
            if (DibujosHandler.Existe(_dibujosService.BuscarDibujoPorIdentificador(dibujoSubir.Identificador)))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que la duración sea válida
            if (archivoDB.Tipo != "img")
            {
                if (!DibujosHandler.DuracionValida(dibujoSubir.TiempoInicio, dibujoSubir.TiempoFin, archivoDB.Duracion))
                {
                    return BadRequest(new { msgError = "La duración del dibujo no es válida." });
                }
            }

            //Creo un guid para el nombre en la nube
            string nombreCloud = Guid.NewGuid().ToString();

            //Creo el objeto y le paso los datos
            Dibujos dibujosDB = new Dibujos()
            {
                IdArchivo = dibujoSubir.IdArchivo,
                NombreCloud = nombreCloud,
                Identificador = dibujoSubir.Identificador,
                TiempoInicio = dibujoSubir.TiempoInicio,
                TiempoFin = dibujoSubir.TiempoFin
            };

            //Lo inserto en la db
            _dibujosService.InsertDibujo(dibujosDB);

            return Ok(new { msgOk = "Dibujo preparado para subir.", NombreCloud = nombreCloud });
        }

        [HttpDelete("delete/{identificador}")]
        public IActionResult Delete(string identificador)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El identificador es valido

            El dibujo existe

            El archivo existe

            El archivo pertenece al usuario
            */

            tokenHandler = new JWTHandler((ClaimsIdentity)User.Identity);

            var claimIdUsuario = tokenHandler.GetId(); //"user_id" del tokenHandler
            var claimSesion = tokenHandler.GetSessionId(); //"session_id" del tokenHandler

            //Traigo al usuario de la DB
            var usuarioDB = _usuariosService.GetUsuario(claimIdUsuario);

            //Comprueba que el usuario exista
            if (!UsuariosHandler.Existe(usuarioDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Compruebo que el identificador sea un guid
            if (!DibujosHandler.IdentificadorValido(identificador))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Traigo al dibujo de la db
            var dibujoDB = _dibujosService.BuscarDibujoPorIdentificador(identificador);

            //Compruebo que el dibujo exista
            if (!DibujosHandler.Existe(dibujoDB))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Traigo al archivo de la DB
            var archivoDB = _archivosService.GetArchivo(dibujoDB.IdArchivo);

            //Compruebo que el archivo exista
            if (!ArchivosHandler.Existe(archivoDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Compruebo que el archivo pertenezca al usuario
            if (!(archivoDB.IdUsuario == claimIdUsuario))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Lo elimino de la db
            _dibujosService.DeleteDibujo(dibujoDB.IdDibujo);

            return Ok(new { msgOk = "El dibujo se eliminó satisfactoriamente." });
        }
    }
}
