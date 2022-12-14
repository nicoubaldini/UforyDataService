using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.Handlers
{
    public class ProyectosHandler : Validation<Proyectos>
    {
        const int _longitudMaximaNombre = 75;
        public static bool NombreValido(string nombre)
        {
            //Comprobar longitud
            if (nombre.Length > _longitudMaximaNombre)
            {
                return false;
            }

            //letras de la A a la Z, mayusculas y minusculas
            Regex letras = new Regex(@"[a-zA-z]");
            //digitos del 0 al 9
            Regex numeros = new Regex(@"[0-9]");

            //si no contiene las letras, numeros o caracteres especiales, retorna false
            if (!letras.IsMatch(nombre) && !numeros.IsMatch(nombre))
            {
                return false;
            }

            return true;
        }
    }
}
