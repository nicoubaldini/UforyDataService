﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.ComentariosRequest
{
    public class ComentariosUpdateImagen
    {
        [Required]
        public int IdArchivo { get; set; }
        [Required]
        public string Contenido { get; set; }
        public int? PosH { get; set; }
        public int? PosV { get; set; }
    }
}
