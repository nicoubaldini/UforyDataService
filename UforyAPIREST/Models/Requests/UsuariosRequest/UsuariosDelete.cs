﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.UsuariosRequest
{
    public class UsuariosDelete
    {
        [Required]
        public string Pass { get; set; }
    }
}
