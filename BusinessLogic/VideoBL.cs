using Api.BlobStorage.Entities;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;

namespace Api.BlobStorage.BusinessLogic
{
    public class VideoBL
    {

        public async Task ReadVideos()
        {
            var bytes1 = System.IO.File.ReadAllBytes("http://127.0.0.1:10000/devstoreaccount1/328260fd02e148558247da79f46e5d9c/video_328260fd02e148558247da79f46e5d9c_1.mp4");
        }

        public async Task<List<BlobDto>> GenerateVideo()
        {

            try
            {
               await  ReadVideos();
                var startInfo = new ProcessStartInfo
                {
                    FileName = @"C:\ffmpeg\ffmpeg.exe",
                    Arguments = "-f concat -i mylist.txt -c copy output4.mp4",
                    WorkingDirectory = @"C:\ffmpeg\",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    process.WaitForExit();
                }
            }catch(Exception ex) { 
            }
            return null;
        }


        public async Task GenerateFile(List<BlobDto> list)
        {

            string blobstorageconnection = System.Configuration.ConfigurationManager.ConnectionStrings["BlobStorage"].ConnectionString;
            string strContainerName = System.Configuration.ConfigurationManager.ConnectionStrings["NameContainer"].ConnectionString;


            BlobContainerClient container = new BlobContainerClient(blobstorageconnection, strContainerName);
            Byte[] bytes;

            foreach(var file in list)
            {

                BlobClient blobClient = container.GetBlobClient(file.Uri);
                if (blobClient.ExistsAsync().Result)
                {
                    using (var ms = new MemoryStream())
                    {
                        blobClient.DownloadTo(ms);
                        bytes = ms.ToArray();

                        using (StreamWriter writer = new StreamWriter(ms))
                        {
                            writer.Write(blobClient.DownloadTo(ms));
                        }
                    }
                }
            }
           


        }
    }
}
