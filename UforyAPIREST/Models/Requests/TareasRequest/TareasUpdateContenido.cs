using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.TareasRequest
{
    public class TareasUpdateContenido
    {
        [Required]
        public string Contenido { get; set; }
    }
}
