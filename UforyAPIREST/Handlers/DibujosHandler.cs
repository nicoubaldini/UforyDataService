using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.Handlers
{
    public class DibujosHandler : Validation<Dibujos>
    {
        public static bool IdentificadorValido(string identificador)
        {
            Guid x;
            return Guid.TryParse(identificador, out x);
        }

        public static bool DuracionValida(int? tiempoInicio, int? tiempoFinal, int? duracionArchivo)
        {
            if (duracionArchivo == null && tiempoInicio == null && tiempoFinal == null)
            {
                return false;
            }

            if (tiempoInicio < 0 || tiempoFinal <= 0)
            {
                return false;
            }

            if (tiempoInicio >= tiempoFinal)
            {
                return false;
            }

            if (tiempoInicio > duracionArchivo || tiempoFinal > duracionArchivo)
            {
                return false;
            }
            return true;
        }

    }
}
