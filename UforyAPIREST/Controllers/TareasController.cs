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
using UforyAPIREST.Models.Requests.TareasRequest;

namespace UforyAPIREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TareasController : Controller
    {
        private readonly ITareasService _tareasService;
        private readonly IProyectosService _proyectosService;
        private readonly IUsuariosService _usuariosService;
        private JWTHandler tokenHandler;

        public TareasController(ITareasService tareasService, IProyectosService proyectosService, IUsuariosService usuariosService)
        {
            _tareasService = tareasService;
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

            var tareasDB = _tareasService.GetTareasEnProyecto(idProyecto);

            return Ok(Converter.LTareaALTareaResponse(tareasDB));
        }

        [HttpPost("create")]
        public IActionResult Post([FromBody] TareasInsert tareaCrear)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El proyecto existe

            El proyecto pertenece al usuario

            El contenido de la tarea es válido

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

            //Compruebo que la suscripcion sea valida
            if (!UsuariosHandler.SuscripcionValida(usuarioDB.Suscripcion))
            {
                return Unauthorized(new { msgError = "La suscripción no es válida." });
            }

            //Compruebo que tenga una suscripcion
            if (!UsuariosHandler.TieneSuscripcion(usuarioDB.Suscripcion))
            {
                return Unauthorized(new { msgError = "Usted necesita una suscripción para subir archivos a la nube." });
            }

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(tareaCrear.IdProyecto);

            //Compruebo que el proyecto exista
            if (!ProyectosHandler.Existe(proyectoDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que le proyecto le pertenezca al usuario
            if (!(proyectoDB.IdUsuario == claimIdUsuario))
            {
                return Unauthorized();
            }

            //Compruebo que el contenido sea valido
            if (!TareasHandler.ContenidoValido(tareaCrear.Contenido))
            {
                return BadRequest(new { msgError = "El contenido de la tarea no es válido." });
            }

            //Traigo todas las tareas del proyecto
            var tareasDB = _tareasService.GetTareasEnProyecto(tareaCrear.IdProyecto);

            int posicion = 0;

            //Pregunto si hay tareas en el proyecto
            if (TareasHandler.HayTareas(tareasDB))
            {
                posicion = TareasHandler.GetPrimeraPosicion(tareasDB) - 1;//Le resto uno a la primera posicion, para que la nueva tarea quede por encima de las demas
            }

            Tareas tareaDB = new Tareas()
            {
                IdProyecto = tareaCrear.IdProyecto,
                Contenido = tareaCrear.Contenido,
                Completada = false,
                Posicion = posicion
            };

            _tareasService.InsertTarea(tareaDB);

            return Ok(new { msgOk = "La tarea se creó satisfactoriamente." });
        }

        [HttpPut("editcontent/{idTarea}")]
        public IActionResult PutContenido([FromBody] TareasUpdateContenido tareaActualizar, int idTarea)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            La tarea existe

            El proyecto existe

            El proyecto pertenece al usuario

            El contenido de la tarea es válido
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

            //Traigo la tarea de la DB
            var tareaDB = _tareasService.GetTarea(idTarea);

            //Compruebo que la tarea exista
            if (!TareasHandler.Existe(tareaDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(tareaDB.IdProyecto);

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

            //Compruebo que el contenido de la tarea sea válido
            if (!TareasHandler.ContenidoValido(tareaActualizar.Contenido))
            {
                return BadRequest(new { msgError = "El contenido de la tarea no es válido." });
            }

            //Actualizo el contenido
            tareaDB.Contenido = tareaActualizar.Contenido;

            _tareasService.UpdateTarea(tareaDB);

            return Ok(new { msgOk = "El contenido de la tarea se actualizó satisfactoriamente." });
        }

        [HttpPut("editposition/{idTarea}")]
        public IActionResult PutPosicion([FromBody] TareasUpdatePosicion tareaActualizar, int idTarea)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            La tarea existe

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

            //Traigo la tarea de la DB
            var tareaDB = _tareasService.GetTarea(idTarea);

            //Compruebo que la tarea exista
            if (!TareasHandler.Existe(tareaDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(tareaDB.IdProyecto);

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

            //Traigo la tarea de la DB
            var tareasDB = _tareasService.GetTareasEnProyecto(tareaDB.IdProyecto);

            //Ordeno la lista
            var tareasOrdenadas = TareasHandler.OrdenarTareas(tareasDB, idTarea, tareaActualizar.NuevaPosicion);

            //Actualizo las tareas
            _tareasService.UpdateListTarea(tareasOrdenadas);

            return Ok(new { msgOk = "Las tareas se ordenaron satisfactoriamente." });
        }

        [HttpPut("editstate/{idTarea}")]
        public IActionResult PutCompletada([FromBody] TareasUpdateCompletada tareaActualizar, int idTarea)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            La tarea existe

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

            //Traigo la tarea de la DB
            var tareaDB = _tareasService.GetTarea(idTarea);

            //Compruebo que la tarea exista
            if (!TareasHandler.Existe(tareaDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(tareaDB.IdProyecto);

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

            tareaDB.Completada = tareaActualizar.Completada;

            //Actualizo
            _tareasService.UpdateTarea(tareaDB);

            return Ok(new { msgOk = "El estado de la tarea sea actualizó satisfactoriamente." });
        }

        [HttpDelete("delete/{idTarea}")]
        public IActionResult Delete(int idTarea)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            La tarea existe

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

            //Traigo la tarea de la DB
            var tareaDB = _tareasService.GetTarea(idTarea);

            //Compruebo que la nota exista
            if (!TareasHandler.Existe(tareaDB))
            {
                return NotFound(new { msgOk = "No fue posible realizar la acción." });
            }

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(tareaDB.IdProyecto);

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
            _tareasService.DeleteTarea(idTarea);

            return Ok(new { msgOk = "La tarea se eliminó satisfactoriamente." });
        }
    }
}
