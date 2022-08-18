using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace UforyAPIREST.Models
{
    public partial class Comentarios
    {
        public int IdComentario { get; set; }
        public int IdArchivo { get; set; }
        public string Identificador { get; set; }
        public string Contenido { get; set; }
        public int PosH { get; set; }
        public int PosV { get; set; }
        public int? TiempoInicio { get; set; }
        public int? TiempoFin { get; set; }
    }
}
