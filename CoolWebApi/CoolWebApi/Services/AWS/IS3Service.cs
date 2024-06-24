using CoolWebApi.Models.Responses.Content;
using CoolWebApi.Utils.Wrapper;
using Microsoft.AspNetCore.Mvc;
using IResult = CoolWebApi.Utils.Wrapper.IResult;

namespace CoolWebApi.Services.AWS
{
    public interface IS3Service : IService
    {
        Task<IResult> UploadFileAsync(IFormFile file, string bucketName, string? prefix);

    }
}