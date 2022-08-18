using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService.Interface
{
    public interface IComentariosService
    {
        public List<Comentarios> GetComentariosEnArchivo(int idArchivo);

        public Comentarios BuscarComentarioPorIdentificador(string identificador);


        public bool InsertComentario(Comentarios nuevoComentario);


        public void UpdateComentario(Comentarios comentarioActualizar);


        public void DeleteComentario(int idComentario);
    }
}
