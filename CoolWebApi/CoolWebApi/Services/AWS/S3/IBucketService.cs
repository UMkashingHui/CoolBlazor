using CoolWebApi.Models.Responses.Content;
using CoolWebApi.Utils.Wrapper;
using Microsoft.AspNetCore.Mvc;
using IResult = CoolWebApi.Utils.Wrapper.IResult;

namespace CoolWebApi.Services.AWS
{
    public interface IBucketService : IService
    {
        Task<IResult> DeleteBucketAsync(string bucketName);

    }
}