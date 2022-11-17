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

        public async Task<List<BlobDto>> GenerateVideo()
        {

            try
            {
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
    }
}
