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
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using CoolWebApi.Models.DTO.AWS;
using IResult = CoolWebApi.Utils.Wrapper.IResult;
using CoolWebApi.Models.DTO.AWS.S3;
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

        public async Task<Result<string>> UploadObjectAsync(UploadObjectDTO dto)
        {
            var _bucketName = dto.BucketName;
            var _filePath = dto.FilePath;
            var _fileName = dto.FileName;
            var _prefix = dto.Prefix;
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

        public async Task<Result<string>> DeleteObjectAsync(string bucketName, string key)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return await Result<string>.FailAsync(_localizer["Bucket {_bucketName} does not exist."]);
            var response = await DeleteObjectNonVersionedBucketAsync(_s3Client, bucketName, key);
            return await Result<string>.SuccessAsync(_localizer[$"File {key} is deleted successfully!"]);
        }

        /// <summary>
        /// The DeleteObjectNonVersionedBucketAsync takes care of deleting the
        /// desired object from the named bucket.
        /// </summary>
        /// <param name="client">An initialized Amazon S3 client used to delete
        /// an object from an Amazon S3 bucket.</param>
        /// <param name="bucketName">The name of the bucket from which the
        /// object will be deleted.</param>
        /// <param name="keyName">The name of the object to delete.</param>
        public static async Task<DeleteObjectResponse> DeleteObjectNonVersionedBucketAsync(IAmazonS3 client, string bucketName, string keyName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                };

                Console.WriteLine($"Deleting object: {keyName}");
                var response = await client.DeleteObjectAsync(deleteObjectRequest);
                Console.WriteLine($"Object: {keyName} deleted from {bucketName}.");
                return response;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' when deleting an object.");
                return null;
            }
        }

    }
}