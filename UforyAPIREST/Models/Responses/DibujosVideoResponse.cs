using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Responses
{
    public class DibujosVideoResponse
    {
        public int IdDibujo { get; set; }
        public string NombreCloud { get; set; }
        public string Identificador { get; set; }
        public int TiempoInicio { get; set; }
        public int TiempoFin { get; set; }
    }
}
