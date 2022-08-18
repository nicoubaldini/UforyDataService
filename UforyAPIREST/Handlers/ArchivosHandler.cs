using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UforyAPIREST.Models;

namespace UforyAPIREST.Handlers
{
    public class ArchivosHandler : Validation<Archivos>
    {
        const long _almLimite = 2147483648; //Almacenamiento límite de 2GB
        const int _longitudMaximaNombre = 259;

        public static bool PuedeSubir(List<Archivos> archivosUsuario, long nuevoArchivoPeso)
        {
            long almDisp = _almLimite;

            foreach (var item in archivosUsuario)
            {
                almDisp -= item.Peso;
            }

            if(nuevoArchivoPeso > almDisp)
            {
                return false;
            }

            return true;
        }

        public static long GetAlmacenamientoUsado(List<Archivos> archivosUsuario)
        {
            long almUsado = 0;
            foreach (var item in archivosUsuario)
            {
                almUsado += item.Peso;
            }

            return almUsado;
        }
        public static bool NombreValido(string nombreArchivo)
        {
            //Comprobar longitud
            if (nombreArchivo.Length > _longitudMaximaNombre)
            {
                return false;
            }

            //caracteres especiales
            Regex caracEsp = new Regex("[\\/:*?\"<>|]");

            //si contiene caracteres especiales no validos
            if (caracEsp.IsMatch(nombreArchivo))
            {
                return false;
            }

            return true;
        }

        public static bool PesoValido(long peso)
        {
            if (peso <= 0)
            {
                return false;
            }
            return true;
        }

        public static bool DuracionValida(int duracion)
        {
            if (duracion > 0)
            {
                return true;
            }
            return false;
        }

        public static bool ResolucionValida(int resH, int resV)
        {

            if (resH > 0 && resV > 0)
            {
                return true;
            }

            return false;
        }

        public static bool ResolucionValidaAudio(int resH, int resV)
        {
            string[] resValidar = { resH.ToString(), resV.ToString() };

            string[,] res = {
                { "1280", "720" },
                { "1920", "1080" },
                { "2560", "1440" }
            };

            if (res[0, 0] == resValidar[0] && res[0, 1] == resValidar[1])
            {
                return true;
            }

            if (res[1, 0] == resValidar[0] && res[1, 1] == resValidar[1])
            {
                return true;
            }

            if (res[2, 0] == resValidar[0] && res[2, 1] == resValidar[1])
            {
                return true;
            }

            return false;
        }

        public static bool EsImagen(string nombreArchivo)
        {
            string ext = GetExt(nombreArchivo);
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                return true;
            }
            return false;
        }

        public static bool EsVideo(string nombreArchivo)
        {
            string ext = GetExt(nombreArchivo);
            if (ext == ".mp4" || ext == ".avi")
            {
                return true;
            }
            return false;
        }

        public static bool EsAudio(string nombreArchivo)
        {
            string ext = GetExt(nombreArchivo);
            if (ext == ".mp3")
            {
                return true;
            }
            return false;
        }

        private static string GetExt(string nombreArchivo)
        {
            string ext = Path.GetExtension(nombreArchivo);

            return ext;
        }
    }
}
