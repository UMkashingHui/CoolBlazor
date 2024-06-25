using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CoolWebApi.Utils.Wrapper;
using CoolWebApi.Models.Responses.Content;
using CoolWebApi.Services.Identity;
using CoolWebApi.Utils.Entities.Catalog;
using CoolWebApi.Utils.Entities.Misc;
using CoolWebApi.Utils.Entities.ExtendedAttributes;
using CoolWebApi.Utils.Repositories;
using CoolWebApi.Services.AWS;
using Amazon.S3;
using CoolWebApi.Models.Requests.AWS;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using CoolWebApi.Models.DTO.AWS;
using IResult = CoolWebApi.Utils.Wrapper.IResult;

namespace CoolWebApi.Services.AWS.impl
{
    public class ObjectService : IObjectService
    {

        private readonly IAmazonS3 _s3Client;

        public ObjectService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<IResult> UploadAsync(IFormFile file, string bucketName, string? prefix)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return await Result.FailAsync($"Bucket {bucketName} does not exist.");
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
            return await Result.SuccessAsync($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
        }

    }
}