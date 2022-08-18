using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.Handlers
{
    public class ComentariosHandler : Validation<Comentarios>
    {
        const int _longitudMaximaContenido = 512;
        const int _longitudIdentificador = 36;
        public static bool ContenidoValido(string contenido)
        {
            //Comprobar longitud
            if (contenido.Length > _longitudMaximaContenido)
            {
                return false;
            }

            //letras de la A a la Z, mayusculas y minusculas
            Regex letras = new Regex(@"[a-zA-z]");
            //digitos del 0 al 9
            Regex numeros = new Regex(@"[0-9]");
            //caracteres especiales
            Regex caracEsp = new Regex("[-*?!@#$/(){}=.,;:•><°]");

            //si no contiene las letras, numeros o caracteres especiales, retorna false
            if (!letras.IsMatch(contenido) && !numeros.IsMatch(contenido) && !caracEsp.IsMatch(contenido))
            {
                return false;
            }

            return true;
        }

        public static bool PosicionValida(int posH, int posV, int? resH, int? resV)
        {
            if (resH == null || resV == null)
            {
                return false;
            }

            if (posH < 0 || posV < 0 || resH < 0 || resV < 0)
            {
                return false;
            }

            if (posH > resH || posV > resV)
            {
                return false;
            }
            return true;
        }

        public static bool DuracionValida(int tiempoInicio, int tiempoFinal, int? duracionArchivo)
        {
            if (duracionArchivo == null)
            {
                return false;
            }

            if (tiempoInicio < 0 || tiempoFinal <= 0)
            {
                return false;
            }

            if (tiempoInicio >= tiempoFinal)
            {
                return false;
            }

            if (tiempoInicio > duracionArchivo || tiempoFinal > duracionArchivo)
            {
                return false;
            }
            return true;
        }

        public static bool IdentificadorValido(string identificador)
        {
            Guid x;
            return Guid.TryParse(identificador, out x);
        }


    }
}
