using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UforyAPIREST.Models.Requests.UsuariosRequest;

namespace UforyAPIREST.Handlers
{
    public class JWTHandler
    {
        readonly ClaimsIdentity _cI;
        public JWTHandler(ClaimsIdentity cI)
        {
            _cI = cI;
        }

        public static string GenerarJWT(string secretKey, int idUsuario, string sesionActual)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);

            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim("user_id", idUsuario.ToString())); //Claim del ID
            claims.AddClaim(new Claim("session_id", sesionActual)); //Claim de la sesion

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(14),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            string bearerToken = tokenHandler.WriteToken(createdToken);

            return bearerToken;
        }

        public int GetId()
        {
            return Int32.Parse(_cI.FindFirst("user_id").Value);
        }

        public string GetSessionId()
        {
            return _cI.FindFirst("session_id").Value;
        }
    }
}
