using CoolWebApi.Models.Requests.AWS.S3;
using CoolWebApi.Models.Responses.Content;
using CoolWebApi.Models.Responses.Identity;
using CoolWebApi.Utils.Wrapper;
using Microsoft.AspNetCore.Mvc;
using IResult = CoolWebApi.Utils.Wrapper.IResult;

namespace CoolWebApi.Services.AWS
{
    public interface IObjectService : IService
    {
        Task<Result<UploadObjectResponse>> UploadObjectAsync(UploadObjectRequest request);
    }
}