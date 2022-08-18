using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.DataService.Interface;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService
{
    public class TareasService : ITareasService
    {
        public List<Tareas> GetTareasEnProyecto(int idProyecto)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Tareas.ToList().FindAll(x => x.IdProyecto == idProyecto);
            }
        }

        public Tareas GetTarea(int idTarea)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Tareas.Find(idTarea);
            }
        }

        public bool InsertTarea(Tareas nuevaTarea)
        {

            using (var dBContext = new UforyDBContext())
            {
                dBContext.Tareas.Add(nuevaTarea);
                dBContext.SaveChanges();
                return true;
            }
        }

        public void UpdateTarea(Tareas tareaActualizar)
        {
            using (var dBContext = new UforyDBContext())
            {
                dBContext.Tareas.Update(tareaActualizar);
                dBContext.SaveChanges();
            }
        }

        public void UpdateListTarea(List<Tareas> tareas)
        {
            using (var dBContext = new UforyDBContext())
            {

                foreach (var item in tareas)
                {
                    dBContext.Tareas.Update(item);
                }
                dBContext.SaveChanges();
            }
        }

        public void DeleteTarea(int idTarea)
        {
            using (var dBContext = new UforyDBContext())
            {
                var archivoEliminar = dBContext.Tareas.Find(idTarea);

                dBContext.Tareas.Remove(archivoEliminar);

                dBContext.SaveChanges();
            }
        }
    }
}
