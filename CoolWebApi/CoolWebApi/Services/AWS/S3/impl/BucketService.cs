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
    public class BucketService : IBucketService
    {

        private readonly IAmazonS3 _s3Client;

        public BucketService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }


        /// <summary>
        /// Shows how to delete an Amazon S3 bucket.
        /// </summary>
        /// <param name="client">An initialized Amazon S3 client object.</param>
        /// <param name="bucketName">The name of the Amazon S3 bucket to delete.</param>
        /// <returns>A boolean value that represents the success or failure of
        /// the delete operation.</returns>
        public async Task<IResult> DeleteBucketAsync(string bucketName)
        {
            var request = new DeleteBucketRequest
            {
                BucketName = bucketName,
            };

            var response = await _s3Client.DeleteBucketAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK ? await Result.SuccessAsync() : await Result.FailAsync();
        }




    }
}