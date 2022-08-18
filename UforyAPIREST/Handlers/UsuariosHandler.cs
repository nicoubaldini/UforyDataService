using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.Handlers
{
    public class UsuariosHandler : Validation<Usuarios>
    {
        const int _longitudMaximaEmail = 75;
        const int _longitudMaximaPass = 50;
        const int _longitudMinimaPass = 6;

        public static bool EmailValido(string dato)
        {
            //Comprobar longitud
            if (dato.Length > _longitudMaximaEmail)
            {
                return false;
            }
            //Validar los caracteres
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(dato, expresion))
            {
                if (Regex.Replace(dato, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool PassValida(string dato)
        {
            //Comprobar longitud
            if (dato.Length < _longitudMinimaPass || dato.Length > _longitudMaximaPass)
            {
                return false;
            }

            //letras de la A a la Z, mayusculas y minusculas
            Regex letras = new Regex(@"[a-zA-z]");
            //digitos del 0 al 9
            Regex numeros = new Regex(@"[0-9]");
            //caracteres especiales
            Regex caracEsp = new Regex("[-*?!@#$/(){}=.,;:]");

            //si no contiene las letras, numeros o caracteres especiales, retorna false
            if (!letras.IsMatch(dato) && !numeros.IsMatch(dato) && !caracEsp.IsMatch(dato))
            {
                return false;
            }

            return true;
        }

        public static bool SuscripcionValida(byte suscripcion)
        {
            //La suscripción debe ser 0, 1 o 2
            if (suscripcion > 2 || suscripcion < 0)
            {
                return false;
            }
            return true;
        }

        public static bool TieneSuscripcion(byte suscripcion)
        {
            if (suscripcion <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
