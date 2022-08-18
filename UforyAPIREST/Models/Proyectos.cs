using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace UforyAPIREST.Models
{
    public partial class Proyectos
    {
        public int IdProyecto { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public DateTime Creacion { get; set; }
        public DateTime UltimaMod { get; set; }
    }
}
