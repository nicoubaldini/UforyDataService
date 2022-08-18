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
using UforyAPIREST.Models.Requests.ProyectosRequest;

namespace UforyAPIREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProyectosController : Controller
    {
        private IProyectosService _proyectosService;
        private IUsuariosService _usuariosService;
        private JWTHandler tokenHandler;

        public ProyectosController(IProyectosService proyectosService, IUsuariosService usuariosService)
        {
            _proyectosService = proyectosService;
            _usuariosService = usuariosService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            */
            tokenHandler = new JWTHandler((ClaimsIdentity)User.Identity);

            var claimIdUsuario = tokenHandler.GetId(); //"user_id" del tokenHandler
            var claimSesion = tokenHandler.GetSessionId(); //"session_id" del tokenHandler

            

            //Traigo al usuario de la DB
            var usuarioDB = _usuariosService.GetUsuario(claimIdUsuario);

            //Comprueba que el usuario exista
            if (!UsuariosHandler.Existe(usuarioDB))
            {
                return BadRequest( new { msgError = "No fue posible realizar la acción."});
            }

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            var proyectosDB = _proyectosService.GetProyectos(claimIdUsuario);

            return Ok(Converter.LProyectoALProyectoResponse(proyectosDB));
        }

        [HttpPost("create")]
        public IActionResult Post([FromBody] ProyectosInsert proyectoCrear)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            El nombre del proyecto es válido
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

            //Verifica que el nombre nuevo sea válido
            if(!ProyectosHandler.NombreValido(proyectoCrear.Nombre))
            {
                return BadRequest(new { msgError = "El nombre del proyecto es inválido." });
            }

            //Creo el proyecto
            Proyectos nuevoProyecto = new Proyectos();
            nuevoProyecto.Nombre = proyectoCrear.Nombre;
            nuevoProyecto.IdUsuario = claimIdUsuario;
            nuevoProyecto.Creacion = DateTime.Now;
            nuevoProyecto.UltimaMod = DateTime.Now;

            _proyectosService.InsertProyecto(nuevoProyecto);

            return Ok(new { msgOk = "El proyecto se creó satisfactoriamente." });
        }

        [HttpPut("editname/{idProyecto}")]
        public IActionResult Put(int idProyecto, [FromBody] ProyectoUpdate proyectoActualizar)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            El proyecto con esa id existe

            El proyecto pertenece al usuario del tokenHandler

            El nombre del proyecto es válido
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

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(idProyecto);

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Verifica que el id del proyecto exista (Que proyectoDB sea distinto de null)
            if (!ProyectosHandler.Existe(proyectoDB))
            {
                return NotFound(new { msgError = "No fue posible encontrar el proyecto." });
            }

            //Verifica que el id del proyecto sea el mismo que el del tokenHandler
            if (proyectoDB.IdUsuario == usuarioDB.IdUsuario)
            {
                return Unauthorized(new { msgError = "No fue posible realizar la acción." });
            }

            //Verifica que el nombre nuevo sea válido
            if (!ProyectosHandler.NombreValido(proyectoActualizar.Nombre))
            {
                return BadRequest(new { msgError = "El nombre del proyecto es inválido." });
            }

            //Actualizo
            proyectoDB.Nombre = proyectoActualizar.Nombre;

            _proyectosService.UpdateProyecto(proyectoDB);

            return Ok(new { msgOk = "El nombre del proyecto fue actualizado satisfactoriamente." });
        }

        [HttpDelete("delete/{idProyecto}")]
        public IActionResult Delete(int idProyecto)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            El proyecto con esa id existe

            El proyecto pertenece al usuario del tokenHandler
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

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(idProyecto);

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Verifica que el id del proyecto exista (Que proyectoDB sea distinto de null)
            if (!ProyectosHandler.Existe(proyectoDB))
            {
                return NotFound(new { msgError = "No fue posible encontrar el proyecto." });
            }

            //Compruebo que el proyecto pertenezca al usuario
            if (!(proyectoDB.IdUsuario == usuarioDB.IdUsuario))
            {
                return Unauthorized(new { msgError = "No fue posible realizar la acción." });
            }

            //Elimino el proyecto
            _proyectosService.DeleteProyecto(idProyecto);

            return Ok(new { msgOk = "El proyecto se eliminó satisfactoriamente." });
        }

    }
}
