using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.NotasRequest
{
    public class NotasUpdateContenido
    {
        [Required]
        public string Contenido { get; set; }
    }
}
