using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService.Interface
{
    public interface IArchivosService
    {
        public List<Archivos> GetArchivosEnProyecto(int idProyecto);

        public List<Archivos> GetArchivosDeUsuario(int idUsuario);

        public Archivos GetArchivo(int idArchivo);

        public bool InsertArchivo(Archivos nuevoArchivo);

        public void UpdateArchivo(Archivos archivoActualizar);

        public void DeleteArchivo(int idArchivo);
    }
}
