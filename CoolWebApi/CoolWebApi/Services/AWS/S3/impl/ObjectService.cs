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
using CoolWebApi.Models.Requests.AWS.S3;
using CoolWebApi.Models.Responses.Identity;

namespace CoolWebApi.Services.AWS.impl
{
    public class ObjectService : IObjectService
    {

        private readonly IAmazonS3 _s3Client;
        private readonly IStringLocalizer<ObjectService> _localizer;


        public ObjectService(
            IAmazonS3 s3Client,
            IStringLocalizer<ObjectService> localizer)
        {
            _s3Client = s3Client;
            _localizer = localizer;
        }

        public async Task<Result<string>> UploadObjectAsync(UploadObjectRequest request)
        {
            var _bucketName = request.BucketName;
            var _filePath = request.FilePath;
            var _fileName = request.FileName;
            var _prefix = request.Prefix;
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(_bucketName);
            if (!bucketExists) return await Result<string>.FailAsync(_localizer["Bucket {_bucketName} does not exist."]);
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = _bucketName,
                FilePath = _filePath,
                Key = string.IsNullOrEmpty(_prefix) ? _fileName : $"{_prefix?.TrimEnd('/')}/{_fileName}",
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };
            var putResponse = await _s3Client.PutObjectAsync(putObjectRequest);
            return await Result<string>.SuccessAsync(_localizer[$"File {_prefix}/{_fileName} uploaded to S3 successfully!"]);
        }

    }
}