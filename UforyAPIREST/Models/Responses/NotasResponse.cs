using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Responses
{
    public class NotasResponse
    {
        public int IdNota { get; set; }
        public string Nombre { get; set; }
        public DateTime UltimaMod { get; set; }
        public string Contenido { get; set; }
    }
}
