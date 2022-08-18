using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Handlers
{
    public class SessionHandler
    {
        public static string GenerarSesion()
        {
            return Guid.NewGuid().ToString();
        }

        public static bool SesionValida(string sesionActual, string sesionToken)
        {
            if(sesionActual == sesionToken)
            {
                return true;
            }
            return false;
        }
    }
}
