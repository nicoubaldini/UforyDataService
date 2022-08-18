using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.UsuariosRequest
{
    public class UsuariosUpdateEmail
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Pass { get; set; }

    }
}
