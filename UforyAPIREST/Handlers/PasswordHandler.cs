using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UforyAPIREST.Handlers
{
    public class PasswordHandler
    {
        const int _longitudSalt = 20;
        public static string GenerarSalt()
        {
            Random obj = new Random();
            string sCadena = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int longitud = sCadena.Length;
            char cletra;
            string sNuevacadena = string.Empty;

            for (int i = 0; i < _longitudSalt; i++)
            {
                cletra = sCadena[obj.Next(longitud)];
                sNuevacadena += cletra.ToString();
            }
            return sNuevacadena;
        }

        public static string GetSHA256(string pass)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(pass));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        public static bool PasswordCorrecta(string passDB, string saltDB, string passFront)
        {
            string passValidar = GetSHA256(passFront + saltDB);

            if (passDB == passValidar)
            {
                return true;
            }
            return false;
        }
    }
}
