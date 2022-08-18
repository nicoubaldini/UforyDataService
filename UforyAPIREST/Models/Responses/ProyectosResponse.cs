using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Responses
{
    public class ProyectosResponse
    {
        public int IdProyecto { get; set; }
        public string Nombre { get; set; }
        public DateTime UltimaMod { get; set; }
    }
}
