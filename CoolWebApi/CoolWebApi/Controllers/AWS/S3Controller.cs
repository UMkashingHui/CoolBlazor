using Amazon.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace CoolWebApi.Controllers.AWS
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class S3Controller : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;

        public S3Controller(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }
    }
}

