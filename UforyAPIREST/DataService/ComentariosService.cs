using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;
using UforyAPIREST.DataService.Interface;

namespace UforyAPIREST.DataService
{
    public class ComentariosService : IComentariosService
    {
        public List<Comentarios> GetComentariosEnArchivo(int idArchivo)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Comentarios.ToList().FindAll(x => x.IdArchivo == idArchivo);
            }
        }

        public bool InsertComentario(Comentarios nuevoComentario)
        {

            using (var dBContext = new UforyDBContext())
            {
                dBContext.Comentarios.Add(nuevoComentario);
                dBContext.SaveChanges();
                return true;
            }
        }

        public void UpdateComentario(Comentarios comentarioActualizar)
        {
            using (var dBContext = new UforyDBContext())
            {
                dBContext.Comentarios.Update(comentarioActualizar);
                dBContext.SaveChanges();
            }
        }

        public void DeleteComentario(int idComentario)
        {
            using (var dBContext = new UforyDBContext())
            {
                var comentarioEliminar = dBContext.Comentarios.Find(idComentario);

                dBContext.Comentarios.Remove(comentarioEliminar);

                dBContext.SaveChanges();
            }
        }

        public Comentarios BuscarComentarioPorIdentificador(string identificador)
        {
            using (var dBContext = new UforyDBContext())
            {
                return dBContext.Comentarios.ToList().FirstOrDefault(x => x.Identificador == identificador);
            }
        }
    }
}
