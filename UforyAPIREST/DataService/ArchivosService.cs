using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.DataService.Interface;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService
{
    public class ArchivosService : IArchivosService
    {
        public List<Archivos> GetArchivosEnProyecto(int idProyecto)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Archivos.ToList().FindAll(x => x.IdProyecto == idProyecto);
            }
        }

        public List<Archivos> GetArchivosDeUsuario(int idUsuario)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Archivos.ToList().FindAll(x => x.IdUsuario == idUsuario);
            }
        }

        public Archivos GetArchivo(int idArchivo)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Archivos.Find(idArchivo);
            }
        }

        public bool InsertArchivo(Archivos nuevoArchivo)
        {

            using (var dBContext = new UforyDBContext())
            {
                dBContext.Archivos.Add(nuevoArchivo);
                dBContext.SaveChanges();
                return true;
            }
        }

        public void UpdateArchivo(Archivos archivoActualizar)
        {
            using (var dBContext = new UforyDBContext())
            {
                dBContext.Archivos.Update(archivoActualizar);
                dBContext.SaveChanges();
            }
        }

        public void DeleteArchivo(int idArchivo)
        {
            using (var dBContext = new UforyDBContext())
            {
                var archivoEliminar = dBContext.Archivos.Find(idArchivo);

                dBContext.Archivos.Remove(archivoEliminar);

                dBContext.SaveChanges();
            }
        }
    }
}
