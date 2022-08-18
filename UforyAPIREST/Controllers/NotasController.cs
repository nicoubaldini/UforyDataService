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
using UforyAPIREST.Models.Requests.NotasRequest;

namespace UforyAPIREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotasController : Controller
    {
        private readonly INotasService _notasService;
        private readonly IProyectosService _proyectosService;
        private readonly IUsuariosService _usuariosService;
        private JWTHandler tokenHandler;

        public NotasController(INotasService notasService, IProyectosService proyectosService, IUsuariosService usuariosService)
        {
            _notasService = notasService;
            _proyectosService = proyectosService;
            _usuariosService = usuariosService;
        }

        [HttpGet("{idProyecto}")]
        public IActionResult Get(int idProyecto)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El proyecto existe

            El proyecto pertenece al usuario

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

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(idProyecto);

            //Compruebo que el proyecto exista
            if (!ProyectosHandler.Existe(proyectoDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que le proyecto le pertenezca al usuario
            if (!(proyectoDB.IdUsuario == claimIdUsuario))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Agarro las notas
            var notasDB = _notasService.GetNotas(idProyecto);

            return Ok(Converter.LNotaALNotaResponse(notasDB));
        }

        [HttpPost("create")]
        public IActionResult Post([FromBody] NotasInsert notaCrear)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            El proyecto existe

            El proyecto pertenece al usuario

            (Si no hay suscripción) No más de tres notas dentro del proyecto

            El nombre de la nota es válida
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

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(notaCrear.IdProyecto);

            //Compruebo que el proyecto exista
            if (!ProyectosHandler.Existe(proyectoDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el proyecto le pertenezca al usuario
            if (!(proyectoDB.IdUsuario == claimIdUsuario))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            if (usuarioDB.Suscripcion < 1)
            {
                if (_notasService.CantidadNotasEnProyecto(notaCrear.IdProyecto) > 2)
                {
                    return Unauthorized(new { msgError = "No fue posible realizar la acción." });
                }
            }

            //Verifica que el nombre nuevo sea válido
            if (!NotasHandler.NombreValido(notaCrear.Nombre))
            {
                return BadRequest(new { msgError = "El nombre del proyecto es inválido." });
            }

            Notas nuevaNota = new Notas()
            {
                IdProyecto = notaCrear.IdProyecto,
                Nombre = notaCrear.Nombre,
                Creacion = DateTime.Now,
                UltimaMod = DateTime.Now,
                Contenido = ""
            };

            _notasService.InsertNota(nuevaNota);

            return Ok(new { msgOk = "La nota se creó satisfactoriamente." });
        }

        [HttpPut("editname/{idNota}")]
        public IActionResult PutNombre(int idNota, [FromBody] NotasUpdateNombre notaActualizar)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            La nota existe

            El proyecto existe

            El proyecto pertenece al usuario

            El nombre de la nota es válido
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

            //Traigo la nota de la DB
            var notaDB = _notasService.GetNota(idNota);

            //Compruebo que la nota exista
            if (!NotasHandler.Existe(notaDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(notaDB.IdProyecto);

            //Compruebo que el proyecto exista
            if (!ProyectosHandler.Existe(proyectoDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el proyecto le pertenezca al usuario
            if (!(proyectoDB.IdUsuario == claimIdUsuario))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Verifica que el nombre nuevo sea válido
            if (!NotasHandler.NombreValido(notaActualizar.Nombre))
            {
                return BadRequest(new { msgError = "El nombre de la nota es inválido." });
            }

            //Actualizo el nombre
            notaDB.Nombre = notaActualizar.Nombre;

            _notasService.UpdateNota(notaDB);

            return Ok(new { msgOk = "El nombre de la nota se actualizó satisfactoriamente." });
        }

        [HttpPut("editcontent/{idNota}")]
        public IActionResult PutContenido(int idNota, [FromBody] NotasUpdateContenido notaActualizar)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            La nota existe

            El proyecto existe

            El proyecto pertenece al usuario

            El contenido de la nota es válido
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

            //Traigo la nota de la DB
            var notaDB = _notasService.GetNota(idNota);

            if (!NotasHandler.Existe(notaDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(notaDB.IdProyecto);

            //Compruebo que el proyecto exista
            if (!ProyectosHandler.Existe(proyectoDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el proyecto le pertenezca al usuario
            if (!(proyectoDB.IdUsuario == claimIdUsuario))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Compruebo que el contenido de la nota sea válido
            if (!NotasHandler.ContenidoValido(notaActualizar.Contenido))
            {
                return BadRequest(new { msgError = "El contenido de la nota no es válido." });
            }

            //Actualizo el contenido
            notaDB.Contenido = notaActualizar.Contenido;
            notaDB.UltimaMod = DateTime.Now;

            _notasService.UpdateNota(notaDB);

            return Ok(new { msgOk = "El contenido de la nota se actualizó satisfactoriamente." });
        }

        [HttpDelete("delete/{idNota}")]
        public IActionResult Delete(int idNota)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            La nota existe

            El proyecto existe

            El proyecto pertenece al usuario
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

            //Traigo la nota de la DB
            var notaDB = _notasService.GetNota(idNota);

            //Compruebo que la nota exista
            if (!NotasHandler.Existe(notaDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(notaDB.IdProyecto);

            //Compruebo que el proyecto exista
            if (!ProyectosHandler.Existe(proyectoDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el proyecto le pertenezca al usuario
            if (!(proyectoDB.IdUsuario == claimIdUsuario))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Elimino la nota
            _notasService.DeleteNota(idNota);

            return Ok(new { msgOk = "La nota se eliminó satisfactoriamente."});
        }
    }
}
