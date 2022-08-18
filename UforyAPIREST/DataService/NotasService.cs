using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;
using UforyAPIREST.DataService.Interface;

namespace UforyAPIREST.DataService
{
    public class NotasService : INotasService
    {
        public List<Notas> GetNotas(int idProyecto)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Notas.ToList().FindAll(x => x.IdProyecto == idProyecto);
            }
        }

        public Notas GetNota(int idNota)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Notas.Find(idNota);
            }
        }

        public int CantidadNotasEnProyecto(int idProyecto)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Notas.ToList().FindAll(x => x.IdProyecto == idProyecto).Count;
            }
        }

        public bool InsertNota(Notas nuevaNota)
        {

            using (var dBContext = new UforyDBContext())
            {
                dBContext.Notas.Add(nuevaNota);
                dBContext.SaveChanges();
                return true;
            }
        }

        public void UpdateNota(Notas notaActualizar)
        {
            using (var dBContext = new UforyDBContext())
            {
                dBContext.Notas.Update(notaActualizar);
                dBContext.SaveChanges();
            }
        }

        public void DeleteNota(int id)
        {
            using (var dBContext = new UforyDBContext())
            {
                var notaEliminar = dBContext.Notas.Find(id);

                dBContext.Notas.Remove(notaEliminar);

                dBContext.SaveChanges();
            }
        }
    }
}
