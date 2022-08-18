using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models
{
    public class Tareas
    {
        public int IdTarea { get; set; }
        public int IdProyecto { get; set; }
        public string Contenido { get; set; }
        public bool Completada { get; set; }
        public int Posicion { get; set; }
    }
}
