using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.ProyectosRequest
{
    public class ProyectoUpdate
    {
        [Required]
        public string Nombre { get; set; }
    }
}
