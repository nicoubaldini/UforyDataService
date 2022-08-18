using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.ComentariosRequest
{
    public class ComentariosInsertAudio
    {
        [Required]
        public int IdArchivo { get; set; }
        [Required]
        public string Identificador { get; set; }
        [Required]
        public string Contenido { get; set; }
        [Required]
        public int PosH { get; set; }
        [Required]
        public int PosV { get; set; }
        [Required]
        public int TiempoInicio { get; set; }
        [Required]
        public int TiempoFin { get; set; }

    }
}
