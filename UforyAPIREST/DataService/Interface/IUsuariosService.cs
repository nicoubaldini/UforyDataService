using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService.Interface
{
    public interface IUsuariosService
    {
        public List<Usuarios> GetAllUsuarios();


        public Usuarios GetUsuario(int id);


        public Usuarios BuscarUsuarioPorEmail(string email);


        public void InsertUsuario(Usuarios nuevoUsuario);


        public void UpdateUsuario(Usuarios nuevosDatos);


        public void DeleteUsuario(int id);

    }
}
