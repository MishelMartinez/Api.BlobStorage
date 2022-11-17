using Api.BlobStorage.BusinessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.BlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly VideoBL videoBL;

        public VideoController()
        {
            videoBL = new VideoBL();
        }
        [HttpPost()]
        [Route("GenerateVideo")]
        public  async Task<int> GenerateVideo()
        {
            try
            {
                 await videoBL.GenerateVideo();
                return 1;
            }catch(Exception ex)
            {
                return 0;
            }
        }
    }
}
