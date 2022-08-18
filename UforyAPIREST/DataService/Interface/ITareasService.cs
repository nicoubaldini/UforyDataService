using System.Collections.Generic;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService.Interface
{
    public interface ITareasService
    {
        public List<Tareas> GetTareasEnProyecto(int idProyecto);

        public Tareas GetTarea(int idTarea);


        public bool InsertTarea(Tareas nuevaTarea);


        public void UpdateTarea(Tareas tareaActualizar);

        public void UpdateListTarea(List<Tareas> tareas);
        public void DeleteTarea(int idTarea);

    }
}