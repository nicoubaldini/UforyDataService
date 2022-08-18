using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.DataService.Interface;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService
{
    public class DibujosService : IDibujosService
    {
        public List<Dibujos> GetDibujosEnArchivo(int idArchivo)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Dibujos.ToList().FindAll(x => x.IdArchivo == idArchivo);
            }
        }

        public Dibujos GetDibujo(int idDibujo)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Dibujos.Find(idDibujo);
            }
        }

        public bool InsertDibujo(Dibujos nuevoDibujo)
        {

            using (var dBContext = new UforyDBContext())
            {
                dBContext.Dibujos.Add(nuevoDibujo);
                dBContext.SaveChanges();
                return true;
            }
        }

        public void UpdateDibujo(Dibujos DibujoActualizar)
        {
            using (var dBContext = new UforyDBContext())
            {
                dBContext.Dibujos.Update(DibujoActualizar);
                dBContext.SaveChanges();
            }
        }

        public void DeleteDibujo(int idDibujo)
        {
            using (var dBContext = new UforyDBContext())
            {
                var DibujoEliminar = dBContext.Dibujos.Find(idDibujo);

                dBContext.Dibujos.Remove(DibujoEliminar);

                dBContext.SaveChanges();
            }
        }

        public Dibujos BuscarDibujoPorIdentificador(string identificador)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Dibujos.ToList().FirstOrDefault(x => x.Identificador == identificador);
            }
        }

    }
}
