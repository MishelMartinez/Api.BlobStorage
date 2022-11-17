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
            }


            return files;
        }

    }
}
