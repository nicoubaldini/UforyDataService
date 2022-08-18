using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Responses
{
    public class ComentariosResponse
    {
        public int IdComentario { get; set; }
        public string Contenido { get; set; }
        public int PosH { get; set; }
        public int PosV { get; set; }
        public int? TiempoInicio { get; set; }
        public int? TiempoFin { get; set; }
    }
}
