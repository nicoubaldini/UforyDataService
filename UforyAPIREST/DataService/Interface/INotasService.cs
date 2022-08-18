using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService.Interface
{
    public interface INotasService
    {
        public List<Notas> GetNotas(int idProyecto);


        public Notas GetNota(int idNota);


        public int CantidadNotasEnProyecto(int idProyecto);


        public bool InsertNota(Notas nuevaNota);


        public void UpdateNota(Notas notaActualizar);


        public void DeleteNota(int id);
    }
}
