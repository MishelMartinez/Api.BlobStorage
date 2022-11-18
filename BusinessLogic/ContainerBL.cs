using Api.BlobStorage.Entities;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System;

namespace Api.BlobStorage.BusinessLogic
{
    public class ContainerBL
    {
        public async Task<List<BlobDto>> GetAllFiles()
        {
            List<BlobDto> files;
            try
            {


                string blobstorageconnection = System.Configuration.ConfigurationManager.ConnectionStrings["BlobStorage"].ConnectionString;

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = System.Configuration.ConfigurationManager.ConnectionStrings["NameContainer"].ConnectionString;
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);



                BlobContainerClient container = new BlobContainerClient(blobstorageconnection, strContainerName);
                files = new List<BlobDto>();
                await foreach (BlobItem file in container.GetBlobsAsync())
                {
                    string uri = container.Uri.ToString();
                    var name = file.Name;
                    var fullUri = $"{uri}/{name}";
                    files.Add(new BlobDto
                    {
                        Uri = fullUri,
                        Name = name,
                        ContentType = file.Properties.ContentType
                    });
                }





            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {

                return null;
            }

            if (files.Count > 0)
            {
                files.Sort(delegate (BlobDto x, BlobDto y)
                {
                    return x.Name.CompareTo(y.Name);

                });
            }


            return files;

        }


        public async Task<List<BlobDto>> GetVideos()
        {
            List<BlobDto> files;
           
            try
            {


                string blobstorageconnection = System.Configuration.ConfigurationManager.ConnectionStrings["BlobStorage"].ConnectionString;

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = System.Configuration.ConfigurationManager.ConnectionStrings["NameContainer"].ConnectionString;
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);

          

                BlobContainerClient container = new BlobContainerClient(blobstorageconnection, strContainerName);
              


                files = new List<BlobDto>();
                
                await foreach (BlobItem file in container.GetBlobsAsync())
                {

                    
                    string uri = container.Uri.ToString();
                    string extension = Path.GetExtension(file.Name);

                    if (extension == ".mp4")
                    {
                        var name = file.Name;
                        var fullUri = $"{uri}/{name}";
                        string numberVideo = name.Split("_")[2];
                        string key = numberVideo.Split(".")[0];
                     
                       
                        
                        files.Add(new BlobDto
                        {
                            Uri = fullUri,
                            Name = name,
                            ContentType = file.Properties.ContentType,
                            NumberVideo = int.Parse(key)
                            
                        });
                    }

                }





            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {

                return null;
            }

            if (files.Count > 0)
            {

              
                files.Sort(delegate (BlobDto x, BlobDto y)
                {
                    return x.NumberVideo.CompareTo(y.NumberVideo);

                });

               await  GenerateFile(files);

            }


            return files;
        }



        public async Task GenerateFile(List<BlobDto> list)
        {

            try
            {
                string blobstorageConnection = System.Configuration.ConfigurationManager.ConnectionStrings["BlobStorage"].ConnectionString;
                string containerName = System.Configuration.ConfigurationManager.ConnectionStrings["NameContainer"].ConnectionString;

                BlobServiceClient blobServiceClient = new BlobServiceClient(blobstorageConnection);
                //Representa el punto de conexión de Blob Storage para la cuenta de almacenamiento.

                BlobContainerClient blobContainerClient = new BlobContainerClient(blobstorageConnection, containerName);

                //Le permite manipular los contenedores de Azure Storage y sus blobs.



              



                using (var ms = new MemoryStream())
                {
                   

                    foreach (var file in list)
                    {

                        BlobClient blobClient = blobContainerClient.GetBlobClient(file.Name);
                        //Le permite manipular los blobs de Azure Storage.
                        // Stream blobStream = blobClient.OpenRead();

                        try
                        {
                            FileStream fileStream = File.OpenWrite(@"C:\Users\Lenovo\Videos\test\file.mp4");
                            await blobClient.DownloadToAsync(fileStream);
                            fileStream.Close();
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                           
                            Console.WriteLine($"No existe directorio: {ex.Message}");
                        }






                       


                        using (var fileStream = System.IO.File.OpenRead($@"C:\Users\Lenovo\Videos\test\{file}.mp4"))
                        {
                            blobClient.Upload(fileStream);
                        }


                        /*

                        if (blobClient.Exists())
                        {
                           
                           

                            using (var temp = new MemoryStream())
                            {
                                blobClient.DownloadTo(temp);
                                Byte[] bytes = temp.ToArray();
                                
                                using (StreamWriter writer = new StreamWriter(ms))
                                {
                                    writer.a(bytes,);
                                }
                              
                                ms.Write(bytes, 0, bytes.Length);
                            }
                            

                            
                        }
                        */

                    }
            //
                    File.WriteAllBytes(@"C:\ffmpeg\resultante1.mp4", ms.ToArray());

                }


            }
            catch (Exception ex)
            {

            }

        }

    }
}
