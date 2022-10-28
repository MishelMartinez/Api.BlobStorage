using Api.BlobStorage.Entities;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Api.BlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageController : ControllerBase
    {
        [HttpGet(Name = "GetFilesBlobStorage")]
        
        public async  Task<List<BlobDto>> GetFiles()
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

            return files;

        }




    }
}
