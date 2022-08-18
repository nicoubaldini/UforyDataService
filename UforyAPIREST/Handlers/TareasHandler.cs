using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.Handlers
{
    public class TareasHandler : Validation<Tareas>
    {
        const int _longitudMaximaContenido = 255;

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

        public static bool HayTareas(List<Tareas> tareas)
        {
            if (tareas.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static int GetPrimeraPosicion(List<Tareas> tareas)
        {
            return tareas.Min(x => x.Posicion);
        }

        public static List<Tareas> OrdenarTareas(List<Tareas> tareasDB, int idTareaActualizar, int nuevaPosicion)
        {
            var tareasOffsetArriba = tareasDB.ToList().FindAll(x => x.Posicion < nuevaPosicion).OrderBy(p => p.Posicion);
            var tareasOffsetAbajo = tareasDB.ToList().FindAll(x => x.Posicion >= nuevaPosicion).OrderBy(p => p.Posicion);

            var pos = nuevaPosicion;

            foreach (var item in tareasOffsetAbajo)
            {
                if(item.IdTarea != idTareaActualizar)
                {
                    pos++;
                    item.Posicion = pos;
                }
            }

            var tareasConcat = tareasOffsetArriba.Concat(tareasOffsetAbajo.ToList());

            tareasConcat.Where(c => c.IdTarea == idTareaActualizar).FirstOrDefault().Posicion = nuevaPosicion;

            return tareasConcat.Cast<Tareas>().ToList();
        }

    }
}
