using System.Collections.Generic;
using UforyAPIREST.Models;

namespace UforyAPIREST.DataService.Interface
{
    public interface IDibujosService
    {
        Dibujos BuscarDibujoPorIdentificador(string identificador);
        void DeleteDibujo(int idDibujo);
        Dibujos GetDibujo(int idDibujo);
        List<Dibujos> GetDibujosEnArchivo(int idArchivo);
        bool InsertDibujo(Dibujos nuevoDibujo);
        void UpdateDibujo(Dibujos DibujoActualizar);
    }
}