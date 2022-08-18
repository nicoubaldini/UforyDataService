using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;
using UforyAPIREST.Models.Requests.UsuariosRequest;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UforyAPIREST.DataService.Interface;
using UforyAPIREST.Handlers;

namespace UforyAPIREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuariosService _usuariosService;
        private JWTHandler tokenHandler;

        public UsuariosController(IConfiguration configuration, IUsuariosService usuariosService)
        {
            _configuration = configuration;
            _usuariosService = usuariosService;
        }

        [HttpGet("profile")]
        public IActionResult Get()
        {
            /*
            VALIDACIONES:

            El usuario con esa id existe

            La sesión del tokenHandler es válida

            */
            tokenHandler = new JWTHandler((ClaimsIdentity)User.Identity);

            var idUsuario = tokenHandler.GetId(); //"user_id" del tokenHandler
            var claimSesion = tokenHandler.GetSessionId(); //"session_id" del tokenHandler

            //Traigo al usuario de la DB
            var usuarioDB = _usuariosService.GetUsuario(idUsuario);

            //Verifica que el id del usuario exista
            if (!UsuariosHandler.Existe(usuarioDB))
            {
                return NotFound(new { msgError = "No fue posible encontrar al usuario." });
            }

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            return Ok(Converter.UsuarioAUsuarioResponse(usuarioDB));
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public IActionResult Post([FromBody] UsuariosInsert usuarioCrear)
        {
            /*
            VALIDACIONES:

            El email ingresado es válido
            El email ingresado no existe

            La password ingresada es válida
            */

            //usuariosService = new UsuariosService();

            //Verifico que el email ingresado sea válido
            if (!UsuariosHandler.EmailValido(usuarioCrear.Email))
            {
                return BadRequest(new { msgError = "El email ingresado no es válido." });
            }

            //Verifico que el email no exista en la DB
            if (UsuariosHandler.Existe(_usuariosService.BuscarUsuarioPorEmail(usuarioCrear.Email)))
            {
                return BadRequest(new { msgError = "El email ingresado ya está registrado." });
            }

            //Verifico que la contraseña ingresada sea válida
            if (!UsuariosHandler.PassValida(usuarioCrear.Pass))
            {
                return BadRequest(new { msgError = "La contraseña ingresada no es válida." });
            }

            //Creo un usuario vacío
            Usuarios usuarioNuevo = new Usuarios();

            //Email
            usuarioNuevo.Email = usuarioCrear.Email;

            //Por defecto, sin suscripción
            usuarioNuevo.Suscripcion = 0;

            //Generar SALT
            usuarioNuevo.SaltPass = PasswordHandler.GenerarSalt();

            //HASH
            usuarioNuevo.Pass = PasswordHandler.GetSHA256(usuarioCrear.Pass + usuarioNuevo.SaltPass);

            //Generar sesion nueva
            usuarioNuevo.Sesion = SessionHandler.GenerarSesion();

            //Insertar usuario
            try
            {
                _usuariosService.InsertUsuario(usuarioNuevo);

                return Ok(new { msgOk = "El usuario se creó correctamente." });
            }
            catch
            {
                return BadRequest(new { msgError = "No fue posible registrar al usuario." });
            }

        }
        [HttpPut("editemail/{idUsuario}")]
        public IActionResult PutEmail(int idUsuario, [FromBody] UsuariosUpdateEmail usuarioActualizarEmail)
        {
            /*
            VALIDACIONES:

            El id es válido
            El id es igual al del tokenHandler

            El usuario con esa id existe

            La sesión del tokenHandler es válida

            El email ingresado es válido
            El email ingresado existe

            La password ingresada es válida
            La password ingresada es correcta
            */

            tokenHandler = new JWTHandler((ClaimsIdentity)User.Identity);

            var claimIdUsuario = tokenHandler.GetId(); //"user_id" del tokenHandler
            var claimSesion = tokenHandler.GetSessionId(); //"session_id" del tokenHandler


            if (!(claimIdUsuario == idUsuario)) //Compruebo que el "user_id" del tokenHandler sea el mismo que el del request
            {
                return Unauthorized();
            }

            //Traigo al usuario de la DB
            var usuarioDB = _usuariosService.GetUsuario(idUsuario);

            //Verifica que el id del usuario exista (Que usuarioDB sea distinto de null)
            if (!UsuariosHandler.Existe(usuarioDB))
            {
                return BadRequest(new { msgError = "No fue posible actualizar los datos." });
            }

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Compruebo que la contraseña sea válida
            if (!UsuariosHandler.PassValida(usuarioActualizarEmail.Pass))
            {
                return BadRequest(new { msgError = "La contraseña ingresada no es válida." });
            }

            //Compruebo que la contraseña sea correcta
            if (!PasswordHandler.PasswordCorrecta(usuarioDB.Pass, usuarioDB.SaltPass, usuarioActualizarEmail.Pass))
            {
                return BadRequest(new { msgError = "La contraseña ingresada no es correcta." });
            }

            //Comprueba que el email nuevo sea valido
            if (!UsuariosHandler.EmailValido(usuarioActualizarEmail.Email))
            {
                return BadRequest(new { msgError = "El email ingresado no es válido." });
            }

            //Verifico que el email nuevo no exista en la DB
            if (UsuariosHandler.Existe(_usuariosService.BuscarUsuarioPorEmail(usuarioActualizarEmail.Email)))
            {
                return BadRequest(new { msgError = "El email ingresado ya está registrado." });
            }

            //Guarda el dato en el objeto
            usuarioDB.Email = usuarioActualizarEmail.Email;

            //Actualizo
            _usuariosService.UpdateUsuario(usuarioDB);

            return Ok(new { msgOk = "El email se actualizó satisfactoriamente." });
        }

        [HttpPut("editpassword/{idUsuario}")]
        public IActionResult PutPassword(int idUsuario, [FromBody] UsuariosUpdatePass usuarioActualizarPass)
        {
            /*
            VALIDACIONES:

            El id es válido
            El id es igual al del tokenHandler

            El usuario con esa id existe

            La sesión del tokenHandler es válida

            La password actual ingresada es válida
            La password nueva es válida

            La password actual ingresada es correcta
            */

            tokenHandler = new JWTHandler((ClaimsIdentity)User.Identity);

            var claimIdUsuario = tokenHandler.GetId(); //"user_id" del tokenHandler
            var claimSesion = tokenHandler.GetSessionId(); //"session_id" del tokenHandler


            if (!(claimIdUsuario == idUsuario)) //Compruebo que el "user_id" del tokenHandler sea el mismo que el del request
            {
                return Unauthorized();
            }

            //Traigo al usuario de la DB
            var usuarioDB = _usuariosService.GetUsuario(idUsuario);

            //Verifica que el id del usuario exista (Que usuarioDB no sea null)
            if (!UsuariosHandler.Existe(usuarioDB))
            {
                return BadRequest(new { msgError = "No fue posible actualizar los datos." });
            }

            //Valido la sesion del tokenHandler
            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Compruebo que la contraseña actual sea válida
            if (!UsuariosHandler.PassValida(usuarioActualizarPass.PassActual))
            {
                return BadRequest(new { msgError = "La contraseña actual ingresada no es válida." });
            }

            //Compruebo que la contraseña nueva sea válida
            if (!UsuariosHandler.PassValida(usuarioActualizarPass.PassNueva))
            {
                return BadRequest(new { msgError = "La contraseña nueva ingresada no es válida." });
            }

            //Compruebo que la contraseña sea correcta
            if (!PasswordHandler.PasswordCorrecta(usuarioDB.Pass, usuarioDB.SaltPass, usuarioActualizarPass.PassActual))
            {
                return BadRequest(new { msgError = "La contraseña ingresada no es correcta." });
            }

            //Generar SALT
            usuarioDB.SaltPass = PasswordHandler.GenerarSalt();

            //HASH
            usuarioDB.Pass = PasswordHandler.GetSHA256(usuarioActualizarPass.PassNueva + usuarioDB.SaltPass);

            //Actualizo
            _usuariosService.UpdateUsuario(usuarioDB);

            return Ok(new { msgOk = "La contraseña se actualizó satisfactoriamente." });
        }

        [HttpDelete("{idUsuario}")]
        public IActionResult Delete(int idUsuario, [FromBody] UsuariosDelete usuarioEliminar)
        {
            /*
            VALIDACIONES:

            El id es válido
            El id es igual al del tokenHandler

            El usuario con esa id existe

            La sesión del tokenHandler es válida
            
            La password ingresada es válida
            La password ingresada es correcta
            */

            tokenHandler = new JWTHandler((ClaimsIdentity)User.Identity);

            var claimIdUsuario = tokenHandler.GetId(); //"user_id" del tokenHandler
            var claimSesion = tokenHandler.GetSessionId(); //"session_id" del tokenHandler

            //Validar tokenHandler

            if (!(claimIdUsuario == idUsuario)) //Compruebo que el "user_id" del tokenHandler sea el mismo que el del request
            {
                return Unauthorized();
            }

            //Traigo al usuario de la DB
            var usuarioDB = _usuariosService.GetUsuario(idUsuario);

            //Verifica que el id del usuario exista
            if (!UsuariosHandler.Existe(usuarioDB))
            {
                return BadRequest(new { msgError = "No se encontró al usuario." });
            }

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion))
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la db
            }

            //Compruebo que la contraseña sea válida
            if (!UsuariosHandler.PassValida(usuarioEliminar.Pass))
            {
                return BadRequest(new { msgError = "La contraseña ingresada es incorrecta." });
            }

            //Compruebo que la contraseña sea correcta
            if (!PasswordHandler.PasswordCorrecta(usuarioDB.Pass, usuarioDB.SaltPass, usuarioEliminar.Pass))
            {
                return BadRequest(new { msgError = "La contraseña ingresada es incorrecta." });
            }

            _usuariosService.DeleteUsuario(idUsuario);

            return Ok(new { msgOk = "El usuario se eliminó satisfactoriamente." });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UsuariosLogin usuarioLogin)
        {
            /*
            VALIDACIONES:

            El email ingresado es válido
            La password ingresada es válida
            
            El email ingresado existe

            La password ingresada es correcta
            */

            List<string> msgErrores = new List<string>();
            
            //Compruebo que el email ingresado sea válido
            if (!UsuariosHandler.EmailValido(usuarioLogin.Email))
            {
                return BadRequest(new { msgError = "El email ingresado no existe." });
            }

            //Compruebo que la contraseña ingresada sae válida
            if (!UsuariosHandler.PassValida(usuarioLogin.Pass))
            {
                return BadRequest(new { msgError = "La contraseña ingresada es incorrecta." });
            }

            //Busco al usuario por el email ingresado
            Usuarios usuarioLoginDB = _usuariosService.BuscarUsuarioPorEmail(usuarioLogin.Email);

            //Compruebo que el email ingresado exista
            if (!UsuariosHandler.Existe(usuarioLoginDB))
            {
                return BadRequest(new { msgError = "El email ingresado no existe." });
            }

            //Compruebo que la contraseña ingresada sea la correcta
            if (!PasswordHandler.PasswordCorrecta(usuarioLoginDB.Pass, usuarioLoginDB.SaltPass, usuarioLogin.Pass))
            {
                return BadRequest(new { msgError = "La contraseña ingresada es incorrecta." });
            }

            //Generar tokenHandler
            var secretKey = _configuration.GetValue<string>("SecretKey"); //Obtengo la SecretKey

            string bearertokenHandler = JWTHandler.GenerarJWT(secretKey, usuarioLoginDB.IdUsuario, usuarioLoginDB.Sesion); //Genero el tokenHandler

            //return Ok(bearertokenHandler);
            return Ok( new { msgOk = "Ha iniciado sesión satisfactoriamente.", tokenHandler = bearertokenHandler} );
        }

        [HttpGet("destroysessions/{idUsuario}")]
        public IActionResult DestroySessions(int idUsuario)
        {
            /*
            VALIDACIONES:

            El id es válido
            El id es igual al del tokenHandler

            El usuario con esa id existe

            La sesión del tokenHandler es válida
            */

            tokenHandler = new JWTHandler((ClaimsIdentity)User.Identity);

            var claimIdUsuario = tokenHandler.GetId(); //"user_id" del tokenHandler
            var claimSesion = tokenHandler.GetSessionId(); //"session_id" del tokenHandler


            if (!(claimIdUsuario == idUsuario)) //Compruebo que el "user_id" del tokenHandler sea el mismo que el del request
            {
                return Unauthorized();
            }

            //Traigo al usuario de la DB
            var usuarioDB = _usuariosService.GetUsuario(idUsuario);

            //Verifica que el id del usuario exista
            if (!UsuariosHandler.Existe(usuarioDB))
            {
                return BadRequest(new { msgError = "No fue posible realizar la acción." });
            }

            if (!SessionHandler.SesionValida(usuarioDB.Sesion, claimSesion)) //Valido la sesion del tokenHandler
            {
                return Unauthorized(); //El "session_id" del tokenHandler es distinto al de la DB
            }

            usuarioDB.Sesion = SessionHandler.GenerarSesion(); //Crea una nueva sesión

            _usuariosService.UpdateUsuario(usuarioDB); //Actualiza el dato de la sesión

            return Ok(new { msgOk = "Las sesiones fueron eliminadas satisfactoriamente." });
        }
    }
}
