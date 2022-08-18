using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UforyAPIREST.DataService.Interface;
using UforyAPIREST.Models;
using UforyAPIREST.Models.Requests.ArchivosRequest;
using UforyAPIREST.Handlers;

namespace UforyAPIREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ArchivosController : Controller
    {
        private readonly IArchivosService _archivosService;
        private readonly IProyectosService _proyectosService;
        private readonly IUsuariosService _usuariosService;
        private JWTHandler tokenHandler;

        public ArchivosController(IArchivosService archivosService, IProyectosService proyectosService, IUsuariosService usuariosService)
        {
            _archivosService = archivosService;
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

            var archivosDB = _archivosService.GetArchivosEnProyecto(idProyecto);

            return Ok(Converter.LArchivoALArchivoResponse(archivosDB));
        }

        [HttpPost("img/upload")]
        public IActionResult PostImagen([FromBody]ArchivosInsertImagen archivoSubir)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            Hay suscripción

            El proyecto existe

            El proyecto pertenece al usuario

            El nombre del archivo es válido
            
            Tiene resh y resv

            El peso es válido

            Hay espacio disponible en la nube
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
            var proyectoDB = _proyectosService.GetProyecto(archivoSubir.IdProyecto);

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

            //Compruebo que el nombre sea valido
            if (!ArchivosHandler.NombreValido(archivoSubir.Nombre))
            {
                return BadRequest(new { msgError = "El nombre del archivo no es válido." });
            }

            //Compruebo que le peso sea valido
            if (!ArchivosHandler.PesoValido(archivoSubir.Peso))
            {
                return BadRequest(new { msgError = "El tamaño del archivo no es válido." });
            }

            //Compruebo que sea una imagen
            if (!ArchivosHandler.EsImagen(archivoSubir.Nombre))
            {
                return BadRequest(new { msgError = "El formato del archivo no es válido." });
            }

            //Compruebo que la resolucion sea valida
            if (!ArchivosHandler.ResolucionValida(archivoSubir.ResH, archivoSubir.ResV))
            {
                return BadRequest(new { msgError = "La resolución del archivo no es válida." });
            }

            //Traigo todos los archivos del usuario
            List<Archivos> archivosDB = _archivosService.GetArchivosDeUsuario(claimIdUsuario);

            //Compruebo que haya espacio disponible
            if (!ArchivosHandler.PuedeSubir(archivosDB, archivoSubir.Peso))
            {
                return Unauthorized(new { msgError = "No queda espacio disponible en la nube." });
            }

            //Creo un guid para el nombre en la nube
            string nombreCloud = Guid.NewGuid().ToString();

            //Creo la varible del archivo
            Archivos archivoDB = new Archivos()
            {
                IdUsuario = claimIdUsuario,
                IdProyecto = archivoSubir.IdProyecto,
                NombreCloud = nombreCloud,
                Tipo = "img",
                Nombre = archivoSubir.Nombre,
                Subida = DateTime.Now,
                UltimaMod = DateTime.Now,
                ResH = archivoSubir.ResH,
                ResV = archivoSubir.ResV,
                Duracion = null,
                Peso = archivoSubir.Peso
            };

            //Guardo en la DB
            _archivosService.InsertArchivo(archivoDB);

            return Ok(new { msgOk = "Archivo preparado para subir.", NombreCloud = nombreCloud });
        }

        [HttpPost("aud/upload")]
        public IActionResult PostAudio([FromBody] ArchivosInsertAudio archivoSubir)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            Hay suscripción

            El proyecto existe

            El proyecto pertenece al usuario

            El nombre del archivo es válido
            
            La resolución es válida

            El peso es válido

            Hay espacio disponible en la nube
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
            var proyectoDB = _proyectosService.GetProyecto(archivoSubir.IdProyecto);

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

            //Compruebo que el nombre sea valido
            if (!ArchivosHandler.NombreValido(archivoSubir.Nombre))
            {
                return BadRequest(new { msgError = "El nombre del archivo no es válido." });
            }

            if(!ArchivosHandler.ResolucionValidaAudio(archivoSubir.ResH, archivoSubir.ResV))
            {
                return BadRequest(new { msgError = "La resolución del archivo no es válida." });
            }

            //Compruebo que le peso sea valido
            if (!ArchivosHandler.PesoValido(archivoSubir.Peso))
            {
                return BadRequest(new { msgError = "El tamaño del archivo no es válido." });
            }

            //Compruebo que sea una imagen
            if (!ArchivosHandler.EsAudio(archivoSubir.Nombre))
            {
                return BadRequest(new { msgError = "El formato del archivo no es válido." });
            }

            //Compruebo que la duracion sea valida
            if(!ArchivosHandler.DuracionValida(archivoSubir.Duracion))
            {
                return BadRequest(new { msgError = "La duración del archivo no es válida." });
            }

            //Traigo todos los archivos del usuario
            List<Archivos> archivosDB = _archivosService.GetArchivosDeUsuario(claimIdUsuario);

            //Compruebo que haya espacio disponible
            if (!ArchivosHandler.PuedeSubir(archivosDB, archivoSubir.Peso))
            {
                return Unauthorized(new { msgError = "No queda espacio disponible en la nube." });
            }

            //Creo un guid para el nombre en la nube
            string nombreCloud = Guid.NewGuid().ToString();

            //Creo la varible del archivo
            Archivos archivoDB = new Archivos()
            {
                IdUsuario = claimIdUsuario,
                IdProyecto = archivoSubir.IdProyecto,
                NombreCloud = nombreCloud,
                Tipo = "aud",
                Nombre = archivoSubir.Nombre,
                Subida = DateTime.Now,
                UltimaMod = DateTime.Now,
                ResH = null,
                ResV = null,
                Duracion = archivoSubir.Duracion,
                Peso = archivoSubir.Peso
            };

            //Guardo en la DB
            _archivosService.InsertArchivo(archivoDB);

            return Ok(new { msgOk = "Archivo preparado para subir.", NombreCloud = nombreCloud });
        }

        [HttpPost("vid/upload")]
        public IActionResult PostVideo([FromBody] ArchivosInsertVideo archivoSubir)
        {
            /*
            VALIDACIONES:

            El usuario existe

            La sesión del tokenHandler es válida

            Hay suscripción

            El proyecto existe

            El proyecto pertenece al usuario

            El nombre del archivo es válido
            
            Tiene resh y resv

            El peso es válido

            Hay espacio disponible en la nube
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
            var proyectoDB = _proyectosService.GetProyecto(archivoSubir.IdProyecto);

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

            //Compruebo que el nombre sea valido
            if (!ArchivosHandler.NombreValido(archivoSubir.Nombre))
            {
                return BadRequest(new { msgError = "El nombre del archivo no es válido." });
            }

            //Compruebo que le peso sea valido
            if (!ArchivosHandler.PesoValido(archivoSubir.Peso))
            {
                return BadRequest(new { msgError = "El tamaño del archivo no es válido." });
            }

            //Compruebo que sea una imagen
            if (!ArchivosHandler.EsVideo(archivoSubir.Nombre))
            {
                return BadRequest(new { msgError = "El formato del archivo no es válido." });
            }

            //Compruebo que la resolucion sea valida
            if (!ArchivosHandler.ResolucionValida(archivoSubir.ResH, archivoSubir.ResV))
            {
                return BadRequest(new { msgError = "La resolución del archivo no es válida." });
            }

            //Compruebo que la duracion sea valida
            if (!ArchivosHandler.DuracionValida(archivoSubir.Duracion))
            {
                return BadRequest(new { msgError = "La duración del archivo no es válida." });
            }

            //Traigo todos los archivos del usuario
            List<Archivos> archivosDB = _archivosService.GetArchivosDeUsuario(claimIdUsuario);

            //Compruebo que haya espacio disponible
            if (!ArchivosHandler.PuedeSubir(archivosDB, archivoSubir.Peso))
            {
                return Unauthorized(new { msgError = "No queda espacio disponible en la nube." });
            }

            //Creo un guid para el nombre en la nube
            string nombreCloud = Guid.NewGuid().ToString();

            //Creo la varible del archivo
            Archivos archivoDB = new Archivos()
            {
                IdUsuario = claimIdUsuario,
                IdProyecto = archivoSubir.IdProyecto,
                NombreCloud = nombreCloud,
                Tipo = "vid",
                Nombre = archivoSubir.Nombre,
                Subida = DateTime.Now,
                UltimaMod = DateTime.Now,
                ResH = archivoSubir.ResH,
                ResV = archivoSubir.ResV,
                Duracion = archivoSubir.Duracion,
                Peso = archivoSubir.Peso
            };

            //Guardo en la DB
            _archivosService.InsertArchivo(archivoDB);

            return Ok(new { msgOk = "Archivo preparado para subir.", NombreCloud = nombreCloud });
        }

        [HttpDelete("delete/{idArchivo}")]
        public IActionResult Delete(int idArchivo)
        {
            /*
            VALIDACIONES:
            El usuario existe

            La sesión del tokenHandler es válida

            El archivo existe

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

            //Traigo el archivo de la DB
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

            //Traigo al proyecto de la DB
            var proyectoDB = _proyectosService.GetProyecto(archivoDB.IdProyecto);

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
            /*
            //Creo el manejador de Blobs y le paso el connection string y nombre del container
            BlobHandler bH = new BlobHandler(_configuration.GetValue<string>("AzureStorage:ConnectionString"), _configuration.GetValue<string>("AzureStorage:Container"));

            try //Elimina el archivo de la nube si existe
            {
                bH.EliminarBlob(archivoDB.NombreCloud);
            } catch { }
            */

            //Elimino el archivo de la db
            _archivosService.DeleteArchivo(idArchivo);

            return Ok(new { msgOk = "El archivo se eliminó satisfactoriamente." });
        }

        
        [HttpGet("usedstorage")]

        public IActionResult GetCloudStorage()
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
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }
            /*
            //Creo el manejador de Blobs y le paso el connection string y nombre del container
            BlobHandler bH = new BlobHandler(_configuration.GetValue<string>("AzureStorage:ConnectionString"), _configuration.GetValue<string>("AzureStorage:Container"));
            */
            //Traigo todos los archivos del usuario

            List<Archivos> archivosDB = _archivosService.GetArchivosDeUsuario(claimIdUsuario);

            /*
            //Paso todos los NombresCloud a una nueva lista
            List<string> nombresCloud = new List<string>();
            foreach (var item in archivosDB)
            {
                nombresCloud.Add(item.NombreCloud);
            }

            //Traigo el almacenamiento usado
            var almUsado = bH.AlmacenamientoUsado(nombresCloud);
            */
            var almUsado = ArchivosHandler.GetAlmacenamientoUsado(archivosDB);

            return Ok( new { AlmacenamientoUsado = almUsado });
        }
    }
}
