using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace UforyAPIREST.Models
{
    public partial class Archivos
    {
        public int IdArchivo { get; set; }
        public int IdProyecto { get; set; }
        public int IdUsuario { get; set; }
        public string NombreCloud { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public DateTime Subida { get; set; }
        public DateTime UltimaMod { get; set; }
        public int? ResH { get; set; }
        public int? ResV { get; set; }
        public int? Duracion { get; set; }
        public long Peso { get; set; }
    }
}
