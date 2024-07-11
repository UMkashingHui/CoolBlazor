using Amazon.S3;
using Amazon.S3.Model;
using CoolWebApi.Models.DTO.AWS;
using CoolWebApi.Models.Requests.AWS.S3;
using CoolWebApi.Models.Responses.Identity;
using CoolWebApi.Services.AWS;
using CoolWebApi.Services.AWS.impl;
using CoolWebApi.Utils.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using IResult = CoolWebApi.Utils.Wrapper.IResult;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace CoolWebApi.Controllers.AWS.S3
{
    // [Authorize]
    [Route("api/aws/s3/object")]
    [ApiController]
    public class ObjectController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;

        private readonly IObjectService _objectService;

        public ObjectController(IAmazonS3 s3Client, IObjectService objectService)
        {
            _s3Client = s3Client;
            _objectService = objectService;
        }

        [HttpPost("upload")]
        public async Task<IResult> UploadFileAsync(UploadObjectRequest request)
        {
            return await _objectService.UploadObjectAsync(request);
        }

        /// <summary>
        /// Get All Objects
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = prefix
            };
            var result = await _s3Client.ListObjectsV2Async(request);
            var s3Objects = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };
                return new S3ObjectDto()
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = _s3Client.GetPreSignedURL(urlRequest),
                };
            });
            return Ok(s3Objects);
        }

        /// <summary>
        /// Get Object By Key
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key"></param>
        /// <returns>Status 200 Ok</returns>
        [HttpGet("get-by-key")]
        public async Task<IActionResult> GetFileByKeyAsync([FromQuery] string bucketName, [FromQuery] string key)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
            return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
        }

        /// <summary>
        /// Delete an Object
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key"></param>
        /// <returns>Status 200 OK</returns>
        [HttpDelete]
        public async Task<IResult> DeleteFileAsync(DeleteS3ObjectRequest request)
        {
            return await _objectService.DeleteObjectAsync(request);
        }
    }
}

