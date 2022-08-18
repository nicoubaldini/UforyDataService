using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Responses
{
    public class ArchivosResponse
    {
        public int IdArchivo { get; set; }
        public string NombreCloud { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public DateTime UltimaMod { get; set; }
        public long Peso { get; set; }
    }
}
