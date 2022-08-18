using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.DibujosRequest
{
    public class DibujosInsert
    {
        public int IdArchivo { get; set; }
        public string Identificador { get; set; }
        public int? TiempoInicio { get; set; }
        public int? TiempoFin { get; set; }
    }
}
