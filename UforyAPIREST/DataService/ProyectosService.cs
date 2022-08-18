using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;
using UforyAPIREST.DataService.Interface;

namespace UforyAPIREST.DataService
{
    public class ProyectosService : IProyectosService
    {
        public List<Proyectos> GetProyectos(int idUsuario)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Proyectos.ToList().FindAll(x => x.IdUsuario == idUsuario);
            }
        }
        public Proyectos GetProyecto(int idProyecto)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Proyectos.Find(idProyecto);
            }
        }

        public void InsertProyecto(Proyectos nuevoProyecto)
        {
            using (var dBContext = new UforyDBContext())
            {
                dBContext.Proyectos.Add(nuevoProyecto);
                dBContext.SaveChanges();
            }
        }

        public void UpdateProyecto(Proyectos nuevosDatos)
        {
            using (var dBContext = new UforyDBContext())
            {
                dBContext.Proyectos.Update(nuevosDatos);
                dBContext.SaveChanges();
            }
        }

        public void DeleteProyecto(int idProyecto)
        {
            using (var dBContext = new UforyDBContext())
            {
                var proyectoEliminar = dBContext.Proyectos.Find(idProyecto);

                dBContext.Proyectos.Remove(proyectoEliminar);

                dBContext.SaveChanges();
            }
        }
    }
}
