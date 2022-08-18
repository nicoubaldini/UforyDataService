using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UforyAPIREST.DataService.Interface;
using UforyAPIREST.Models;
using UforyAPIREST.Models.Requests.ComentariosRequest;
using UforyAPIREST.Handlers;

namespace UforyAPIREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComentariosController : Controller
    {
        private readonly IArchivosService _archivosService;
        private readonly IUsuariosService _usuariosService;
        private readonly IComentariosService _comentariosService;
        private readonly IDibujosService _dibujosService;
        private JWTHandler tokenHandler;

        public ComentariosController(IArchivosService archivosService, IUsuariosService usuariosService, IComentariosService comentariosService)
        {
            _archivosService = archivosService;
            _usuariosService = usuariosService;
            _comentariosService = comentariosService;
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

            var comentariosDB = _comentariosService.GetComentariosEnArchivo(idArchivo);

            return Ok(Converter.LComentarioALComentarioResponse(comentariosDB));
        }

        [HttpPost("img/create")]
        public IActionResult PostComentarioImagen([FromBody] ComentariosInsertImagen comentarioCrear)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El archivo existe

            El archivo pertenece al usuario

            El tipo es "img"

            El identificador es valido

            El identificador no pertenece a ningun comentario

            El contenido es valido

            La posicion es valida
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
            var archivoDB = _archivosService.GetArchivo(comentarioCrear.IdArchivo);

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

            //Compruebo que el archivo sea una imagen
            if (archivoDB.Tipo != "img")
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el identificador sea un guid
            if (!ComentariosHandler.IdentificadorValido(comentarioCrear.Identificador))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que no exista un comentario con ese identificador
            if (ComentariosHandler.Existe(_comentariosService.BuscarComentarioPorIdentificador(comentarioCrear.Identificador)))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el contenido sea valido
            if (!ComentariosHandler.ContenidoValido(comentarioCrear.Contenido))
            {
                return BadRequest(new { msgError = "El contenido del comentario no es válido." });
            }

            //Se valida la posicion en base a la resolución del archivo
            if (!ComentariosHandler.PosicionValida(comentarioCrear.PosH, comentarioCrear.PosV, (int)archivoDB.ResH, (int)archivoDB.ResV))
            {
                return BadRequest(new { msgError = "La posición del contenido no es válida." });
            }

            //Creo el objeto y le paso los datos
            Comentarios comentarioDB = new Comentarios()
            {
                IdArchivo = comentarioCrear.IdArchivo,
                Identificador = comentarioCrear.Identificador,
                Contenido = comentarioCrear.Contenido,
                PosH = comentarioCrear.PosH,
                PosV = comentarioCrear.PosV,
                TiempoInicio = null,
                TiempoFin = null
            };

            //Lo inserto en la db
            _comentariosService.InsertComentario(comentarioDB);

            return Ok(new { msgOk = "El comentario se creó satisfactoriamente." });
        }

        [HttpPost("aud/create")]
        public IActionResult PostComentarioAudio([FromBody] ComentariosInsertAudio comentarioCrear)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El archivo existe

            El archivo pertenece al usuario

            El tipo es "aud"

            El identificador es valido

            El identificador no pertenece a ningun comentario

            El contenido es valido

            La posicion es valida

            La duracion es  valida
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
            var archivoDB = _archivosService.GetArchivo(comentarioCrear.IdArchivo);

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

            //Compruebo que el archivo sea una imagen
            if (archivoDB.Tipo != "aud")
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el identificador sea un guid
            if (!ComentariosHandler.IdentificadorValido(comentarioCrear.Identificador))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que no exista un comentario con ese identificador
            if (ComentariosHandler.Existe(_comentariosService.BuscarComentarioPorIdentificador(comentarioCrear.Identificador)))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el contenido sea valido
            if (!ComentariosHandler.ContenidoValido(comentarioCrear.Contenido))
            {
                return BadRequest(new { msgError = "El contenido del comentario no es válido." });
            }

            //Se valida la posicion en base a la resolución del archivo
            //Obtener la resolucion del dibujo en cuanto se implemente
            if (!ComentariosHandler.PosicionValida(comentarioCrear.PosH, comentarioCrear.PosV, 0, 0))
            {
                return BadRequest(new { msgError = "La posición del contenido no es válida." });
            }

            //Se valida la posicion en base a la resolución del archivo
            if (!ComentariosHandler.DuracionValida(comentarioCrear.TiempoInicio, comentarioCrear.TiempoFin, archivoDB.Duracion))
            {
                return BadRequest(new { msgError = "La duración del archivo no es válida." });
            }

            //Creo el objeto y le paso los datos
            Comentarios comentarioDB = new Comentarios()
            {
                IdArchivo = comentarioCrear.IdArchivo,
                Identificador = comentarioCrear.Identificador,
                Contenido = comentarioCrear.Contenido,
                PosH = comentarioCrear.PosH,
                PosV = comentarioCrear.PosV,
                TiempoInicio = null,
                TiempoFin = null
            };

            //Lo inserto en la db
            _comentariosService.InsertComentario(comentarioDB);

            return Ok(new { msgOk = "El comentario se creó satisfactoriamente." });
        }

        [HttpPost("vid/create")]
        public IActionResult PostComentarioVideo([FromBody] ComentariosInsertVideo comentarioCrear)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El archivo existe

            El archivo pertenece al usuario

            El tipo es "vid"

            El identificador es valido

            El identificador no pertenece a ningun comentario

            El contenido es valido

            La posicion es valida

            La duracion es  valida
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
            var archivoDB = _archivosService.GetArchivo(comentarioCrear.IdArchivo);

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

            //Compruebo que el archivo sea una imagen
            if (archivoDB.Tipo != "vid")
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el identificador sea un guid
            if (!ComentariosHandler.IdentificadorValido(comentarioCrear.Identificador))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que no exista un comentario con ese identificador
            if(ComentariosHandler.Existe(_comentariosService.BuscarComentarioPorIdentificador(comentarioCrear.Identificador)))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el contenido sea valido
            if (!ComentariosHandler.ContenidoValido(comentarioCrear.Contenido))
            {
                return BadRequest(new { msgError = "El contenido del comentario no es válido." });
            }

            //Se valida la posicion en base a la resolución del archivo
            if (!ComentariosHandler.PosicionValida(comentarioCrear.PosH, comentarioCrear.PosV, archivoDB.ResH, archivoDB.ResV))
            {
                return BadRequest(new { msgError = "La posición del contenido no es válida." });
            }

            //Se valida la posicion en base a la resolución del archivo
            if (!ComentariosHandler.DuracionValida(comentarioCrear.TiempoInicio, comentarioCrear.TiempoFin, archivoDB.Duracion))
            {
                return BadRequest(new { msgError = "La duración del archivo no es válida." });
            }

            //Creo el objeto y le paso los datos
            Comentarios comentarioDB = new Comentarios()
            {
                IdArchivo = comentarioCrear.IdArchivo,
                Identificador = comentarioCrear.Identificador,
                Contenido = comentarioCrear.Contenido,
                PosH = comentarioCrear.PosH,
                PosV = comentarioCrear.PosV,
                TiempoInicio = comentarioCrear.TiempoInicio,
                TiempoFin = comentarioCrear.TiempoFin
            };

            //Lo inserto en la db
            _comentariosService.InsertComentario(comentarioDB);

            return Ok(new { msgOk = "El comentario se creó satisfactoriamente." });
        }

        [HttpPut("img/edit/{identificador}")]
        public IActionResult PutComentarioImagen(string identificador, [FromBody]ComentariosUpdateImagen comentarioActualizar)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El archivo existe

            El archivo pertenece al usuario

            El tipo es "img"

            El identificador es valido

            El comentario existe

            El comentario pertenece al archivo

            (Si el contenido es distinto de null)
            El contenido es valido

            (Si la posición es distinta de null)
            La posicion es valida

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
            var archivoDB = _archivosService.GetArchivo(comentarioActualizar.IdArchivo);

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

            //Compruebo que el archivo sea una imagen
            if (archivoDB.Tipo != "img")
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el identificador sea un guid
            if (!ComentariosHandler.IdentificadorValido(identificador))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Traigo al comentario de la db
            var comentarioDB = _comentariosService.BuscarComentarioPorIdentificador(identificador);

            //Compruebo que el comentario exista
            if (!ComentariosHandler.Existe(comentarioDB))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el comentario pertenezca al archivo
            if (comentarioDB.IdArchivo != comentarioActualizar.IdArchivo)
            {
                return Unauthorized();
            }

            //Si el comentario es distinto de null
            if (comentarioActualizar.Contenido != null)
            {
                //Compruebo que el contenido sea valido
                if (!ComentariosHandler.ContenidoValido(comentarioActualizar.Contenido))
                {
                    return BadRequest(new { msgError = "El contenido del comentario no es válido." });
                }

                //Si el dato existe y es válido, lo paso al objeto
                comentarioDB.Contenido = comentarioActualizar.Contenido;
            }

            //Si las posiciones son distintas de null
            if(comentarioActualizar.PosH != null && comentarioActualizar.PosV != null)
            {
                //Se valida la posicion en base a la resolución del archivo
                if (!ComentariosHandler.PosicionValida((int)comentarioActualizar.PosH, (int)comentarioActualizar.PosV, (int)archivoDB.ResH, (int)archivoDB.ResV))
                {
                    return BadRequest(new { msgError = "La posición del contenido no es válida." });
                }

                //Si el dato existe y es válido, lo paso al objeto
                comentarioDB.PosH = (int)comentarioActualizar.PosH;
                comentarioDB.PosV = (int)comentarioActualizar.PosV;
            }

            //Lo actualizo en la db
            _comentariosService.UpdateComentario(comentarioDB);

            return Ok(new { msgOk = "El comentario se actualizó satisfactoriamente." });
        }

        [HttpPut("aud/edit/{identificador}")]
        public IActionResult PutComentarioAudio(string identificador, [FromBody] ComentariosUpdateAudio comentarioActualizar)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El archivo existe

            El archivo pertenece al usuario

            El tipo es "img"

            El identificador es valido

            El comentario existe

            El comentario pertenece al archivo

            (Si el contenido es distinto de null)
            El contenido es valido

            (Si la posición es distinta de null)
            La posicion es valida

            (Si la duracion es distinta de null)
            La duracion es valida

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
            var archivoDB = _archivosService.GetArchivo(comentarioActualizar.IdArchivo);

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

            //Compruebo que el archivo sea una imagen
            if (archivoDB.Tipo != "aud")
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el identificador sea un guid
            if (!ComentariosHandler.IdentificadorValido(identificador))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Traigo al comentario de la db
            var comentarioDB = _comentariosService.BuscarComentarioPorIdentificador(identificador);

            //Compruebo que el comentario exista
            if (!ComentariosHandler.Existe(comentarioDB))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el comentario pertenezca al archivo
            if (comentarioDB.IdArchivo != comentarioActualizar.IdArchivo)
            {
                return Unauthorized();
            }

            //Si el comentario es distinto de null
            if (comentarioActualizar.Contenido != null)
            {
                //Compruebo que el contenido sea valido
                if (!ComentariosHandler.ContenidoValido(comentarioActualizar.Contenido))
                {
                    return BadRequest(new { msgError = "El contenido del comentario no es válido." });
                }

                //Si el dato existe y es válido, lo paso al objeto
                comentarioDB.Contenido = comentarioActualizar.Contenido;
            }

            //Si las posiciones son distintas de null
            if (comentarioActualizar.PosH != null && comentarioActualizar.PosV != null)
            {
                //Se obtiene la resolucion del lienzo

                //Se valida la posicion en base a la resolución del archivo
                if (!ComentariosHandler.PosicionValida((int)comentarioActualizar.PosH, (int)comentarioActualizar.PosV, 0, 0))
                {
                    return BadRequest(new { msgError = "La posición del contenido no es válida." });
                }

                //Si el dato existe y es válido, lo paso al objeto
                comentarioDB.PosH = (int)comentarioActualizar.PosH;
                comentarioDB.PosV = (int)comentarioActualizar.PosV;
            }
            //Agarro los tiempos solo si son distintos de null
            if(comentarioActualizar.TiempoInicio != null)
            {
                comentarioDB.TiempoInicio = comentarioActualizar.TiempoInicio;
            }

            if (comentarioActualizar.TiempoFin != null)
            {
                comentarioDB.TiempoFin = comentarioActualizar.TiempoFin;
            }

            //Valido los tiempos
            if(!ComentariosHandler.DuracionValida((int)comentarioDB.TiempoInicio, (int)comentarioDB.TiempoFin, archivoDB.Duracion))
            {
                return BadRequest(new { msgError = "La duración del comentario no es válida." });
            }

            //Lo actualizo en la db
            _comentariosService.UpdateComentario(comentarioDB);

            return Ok(new { msgOk = "El comentario se actualizó satisfactoriamente." });
        }

        [HttpPut("vid/edit/{identificador}")]
        public IActionResult PutComentarioVideo(string identificador, [FromBody] ComentariosUpdateVideo comentarioActualizar)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El archivo existe

            El archivo pertenece al usuario

            El tipo es "img"

            El identificador es valido

            El comentario existe

            El comentario pertenece al archivo

            (Si el contenido es distinto de null)
            El contenido es valido

            (Si la posición es distinta de null)
            La posicion es valida

            (Si la duracion es distinta de null)
            La duracion es valida

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
            var archivoDB = _archivosService.GetArchivo(comentarioActualizar.IdArchivo);

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

            //Compruebo que el archivo sea una imagen
            if (archivoDB.Tipo != "vid")
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el identificador sea un guid
            if (!ComentariosHandler.IdentificadorValido(identificador))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Traigo al comentario de la db
            var comentarioDB = _comentariosService.BuscarComentarioPorIdentificador(identificador);

            //Compruebo que el comentario exista
            if (!ComentariosHandler.Existe(comentarioDB))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Compruebo que el comentario pertenezca al archivo
            if (comentarioDB.IdArchivo != comentarioActualizar.IdArchivo)
            {
                return Unauthorized();
            }

            //Si el comentario es distinto de null
            if (comentarioActualizar.Contenido != null)
            {
                //Compruebo que el contenido sea valido
                if (!ComentariosHandler.ContenidoValido(comentarioActualizar.Contenido))
                {
                    return BadRequest(new { msgError = "El contenido del comentario no es válido." });
                }

                //Si el dato existe y es válido, lo paso al objeto
                comentarioDB.Contenido = comentarioActualizar.Contenido;
            }

            //Si las posiciones son distintas de null
            if (comentarioActualizar.PosH != null && comentarioActualizar.PosV != null)
            {
                //Se valida la posicion en base a la resolución del archivo
                if (!ComentariosHandler.PosicionValida((int)comentarioActualizar.PosH, (int)comentarioActualizar.PosV, archivoDB.ResH, archivoDB.ResV))
                {
                    return BadRequest(new { msgError = "La posición del contenido no es válida." });
                }

                //Si el dato existe y es válido, lo paso al objeto
                comentarioDB.PosH = (int)comentarioActualizar.PosH;
                comentarioDB.PosV = (int)comentarioActualizar.PosV;
            }


            //Agarro los tiempos solo si son distintos de null
            if (comentarioActualizar.TiempoInicio != null)
            {
                comentarioDB.TiempoInicio = comentarioActualizar.TiempoInicio;
            }

            if (comentarioActualizar.TiempoFin != null)
            {
                comentarioDB.TiempoFin = comentarioActualizar.TiempoFin;
            }

            //Valido los tiempos
            if (!ComentariosHandler.DuracionValida((int)comentarioDB.TiempoInicio, (int)comentarioDB.TiempoFin, archivoDB.Duracion))
            {
                return BadRequest(new { msgError = "La duración del comentario no es válida." });
            }

            //Lo actualizo en la db
            _comentariosService.UpdateComentario(comentarioDB);

            return Ok(new { msgOk = "El comentario se actualizó satisfactoriamente." });
        }

        [HttpDelete("delete/{identificador}")]
        public IActionResult Delete(string identificador)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            El identificador es valido

            El comentario existe

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
            if (!ComentariosHandler.IdentificadorValido(identificador))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Traigo al comentario de la db
            var comentarioDB = _comentariosService.BuscarComentarioPorIdentificador(identificador);

            //Compruebo que el comentario exista
            if (!ComentariosHandler.Existe(comentarioDB))
            {
                return NotFound(new { msgError = "No fue posible realizar la acción." });
            }

            //Traigo al archivo de la DB
            var archivoDB = _archivosService.GetArchivo(comentarioDB.IdArchivo);

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
            _comentariosService.DeleteComentario(comentarioDB.IdComentario);

            return Ok(new { msgOk = "El comentario se eliminó satisfactoriamente." });
        }
    }
}
