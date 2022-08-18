using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Responses
{
    public class TareasResponse
    {
        public int IdTarea { get; set; }
        public string Contenido { get; set; }
        public bool Completada { get; set; }
        public int Posicion { get; set; }
    }
}
