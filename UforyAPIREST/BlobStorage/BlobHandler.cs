using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UforyAPIREST.BlobStorage
{
    public class BlobHandler
    {
        BlobServiceClient blobServiceClient;
        BlobContainerClient containerClient;
        const long _almLimite = 2147483648; //Almacenamiento límite de 2GB

        public BlobHandler(string constring, string containerName)
        {
            blobServiceClient = new BlobServiceClient(constring);

            containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public long AlmacenamientoUsado(List<string> nombresCloud)
        {
            long almUsado = 0;

            if(nombresCloud == null)
            {
                return almUsado;
            }

            var blobsUsuario = GetBlobsDeUsuario(nombresCloud);

            //Resto el peso de los blobs al almacenamiento limite
            foreach (var item in blobsUsuario)
            {
                if (item != null)
                {
                    almUsado += (long)item.Properties.ContentLength;
                }
            }

            return almUsado;
        }

        public long AlmacenamientoDisponible(List<string> nombresCloud)
        {
            long almDisp = _almLimite;

            if (nombresCloud == null)
            {
                return almDisp;
            }

            var blobsUsuario = GetBlobsDeUsuario(nombresCloud);


            //Resto el peso de los blobs al almacenamiento limite
            foreach (var item in blobsUsuario)
            {
                if(item != null)
                {
                    almDisp -= (long)item.Properties.ContentLength;
                }
            }
            

            return almDisp;
        }

        private List<BlobItem> GetBlobsDeUsuario(List<string> nombresCloud)
        {
            //Todos los blobs del contenedor
            List<BlobItem> blobsUsuario = new List<BlobItem>();

            //Busco los ETags dentro de la lista de blobs
            foreach (var item in nombresCloud)
            {
                blobsUsuario.Add(containerClient.GetBlobs().ToList().FirstOrDefault(x => x.Name == item));
            }

            return blobsUsuario;
        }

        public bool PuedeSubir(List<string> nombresCloud, long pesoArchivo)
        {

            if (pesoArchivo > AlmacenamientoDisponible(nombresCloud))
            {
                return false;
            }

            return true;
        }

        public void EliminarBlob(string nombreBlob)
        {
            containerClient.DeleteBlob(nombreBlob);

        }

        /*public void subir()
        {
            string localPath = ".";
            string fileName = "quickstart" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = System.IO.Path.Combine(localPath, fileName);

            // Write text to the file
            System.IO.File.WriteAllTextAsync(localFilePath, "Hello, World!");

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            IDictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("identifier", Guid.NewGuid().ToString());

            
            blobClient.SetMetadata(metadata);

            using System.IO.FileStream uploadFileStream = System.IO.File.OpenRead(localFilePath);
            
            blobClient.Upload(uploadFileStream, true);
            uploadFileStream.Close();

            //blobClient.SetMetadata(metadata, null, default);
        }*/

        /*public void descargar()
        {
            BlobClient blobClient = containerClient.GetBlobClient("1_2.jpg");

            BlobDownloadInfo download = blobClient.Download();

            string localPath = ".";
            string fileName = "quickstart" + Guid.NewGuid().ToString() + ".jpg";
            string localFilePath = System.IO.Path.Combine(localPath, fileName);
            //string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");

            using (System.IO.FileStream downloadFileStream = System.IO.File.OpenWrite(localFilePath))
            {
                download.Content.CopyTo(downloadFileStream);
                downloadFileStream.Close();
            }
        }*/

       

    }
}
