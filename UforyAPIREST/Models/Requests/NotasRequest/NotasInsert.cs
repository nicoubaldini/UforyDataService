using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.NotasRequest
{
    public class NotasInsert
    {
        [Required]
        public int IdProyecto { get; set; }
        [Required]
        public string Nombre { get; set; }
    }
}
