using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.Handlers
{
    public class Validation<T>
    {
        public static bool Existe(T t)
        {
            if(t != null)
            {
                return true;
            }
            return false;
        }
    }
}
