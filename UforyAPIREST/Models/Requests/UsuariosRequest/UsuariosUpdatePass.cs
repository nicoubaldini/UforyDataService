using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Models.Requests.UsuariosRequest
{
    public class UsuariosUpdatePass
    {
        [Required]
        public string PassActual { get; set; }
        [Required]
        public string PassNueva { get; set; }

    }
}
