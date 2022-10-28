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
        [HttpGet()]
        [Route ("GetFilesBlobStorage")]
        
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


        [HttpPost()]
        [Route("UploadFile")]

        public async Task Upload(IFormFile files)
        {

            try
            {
                byte[] dataFiles;
                
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.ConnectionStrings["BlobStorage"].ConnectionString);
              
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
              
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(System.Configuration.ConfigurationManager.ConnectionStrings["NameContainer"].ConnectionString);

                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                string systemFileName = files.FileName;
                await cloudBlobContainer.SetPermissionsAsync(permissions);
                await using (var target = new MemoryStream())
                {
                    files.CopyTo(target);
                    dataFiles = target.ToArray();
                }
                
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(systemFileName);
                await cloudBlockBlob.UploadFromByteArrayAsync(dataFiles, 0, dataFiles.Length);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost()]
        [Route("GetFilesForPrefix/{prefix}")]

        public async Task<List<BlobDto>> GetFilesForPrefix(string prefix)
        {
            List<BlobDto> files = new List<BlobDto>();
            try
            {


                string blobstorageconnection = System.Configuration.ConfigurationManager.ConnectionStrings["BlobStorage"].ConnectionString;
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = System.Configuration.ConfigurationManager.ConnectionStrings["NameContainer"].ConnectionString;
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);



                BlobContainerClient container = new BlobContainerClient(blobstorageconnection, strContainerName);

                await foreach (BlobItem file in container.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, default))
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



        [HttpPost()]
        [Route("GetFilesForNumber/{number}")]
        public async Task<List<BlobDto>> GetFilesForNumber(int number)
        {
            List<BlobDto> files = new List<BlobDto>();
            try
            {


                string blobstorageconnection = System.Configuration.ConfigurationManager.ConnectionStrings["BlobStorage"].ConnectionString;
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = System.Configuration.ConfigurationManager.ConnectionStrings["NameContainer"].ConnectionString;
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);


                BlobContinuationToken blobContinuationToken = null;
                var resultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(
                      prefix: null,
                      useFlatBlobListing: true,
                      blobListingDetails: BlobListingDetails.None,
                      maxResults: number,
                      currentToken: blobContinuationToken,
                      options: null,
                      operationContext: null
                  );




                foreach (IListBlobItem item in resultSegment.Results)
                {
                    files.Add(new BlobDto
                    {
                        Uri = item.Uri.ToString(),


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

