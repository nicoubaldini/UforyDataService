using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;
using UforyAPIREST.DataService.Interface;

namespace UforyAPIREST.DataService
{
    public class UsuariosService : IUsuariosService
    {
        public List<Usuarios> GetAllUsuarios()
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Usuarios.ToList();
            }
        }

        public Usuarios GetUsuario(int id)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Usuarios.Find(id);
            }
        }

        public Usuarios BuscarUsuarioPorEmail(string email)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Usuarios.ToList().FirstOrDefault(x => x.Email == email);
            }
        }

        public void InsertUsuario(Usuarios nuevoUsuario)
        {
            using (var dBContext = new UforyDBContext())
            {
                dBContext.Usuarios.Add(nuevoUsuario);
                dBContext.SaveChanges();
            }
        }

        public void UpdateUsuario(Usuarios nuevosDatos)
        {
            using (var dBContext = new UforyDBContext())
            {
                dBContext.Usuarios.Update(nuevosDatos);
                dBContext.SaveChanges();
            }
        }

        public void DeleteUsuario(int id)
        {
            using (var dBContext = new UforyDBContext())
            {
                var usuarioEliminar = dBContext.Usuarios.Find(id);

                dBContext.Usuarios.Remove(usuarioEliminar);

                dBContext.SaveChanges();
            }
        }
    }
}
