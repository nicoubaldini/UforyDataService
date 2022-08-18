using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService.Interface
{
    public interface IProyectosService
    {
        public List<Proyectos> GetProyectos(int idUsuario);
        public Proyectos GetProyecto(int idProyecto);

        public void InsertProyecto(Proyectos nuevoProyecto);

        public void UpdateProyecto(Proyectos nuevosDatos);


        public void DeleteProyecto(int idProyecto);

    }
}
