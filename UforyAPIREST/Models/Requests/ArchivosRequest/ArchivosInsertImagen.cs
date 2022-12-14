using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.ArchivosRequest
{
    public class ArchivosInsertImagen
    {
        [Required]
        public int IdProyecto { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public int ResH { get; set; }
        [Required]
        public int ResV { get; set; }
        [Required]
        public long Peso { get; set; }
    }
}
