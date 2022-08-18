using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace UforyAPIREST.Models
{
    public partial class Usuarios
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public byte Suscripcion { get; set; }
        public string SaltPass { get; set; }
        public string Sesion { get; set; }
    }
}