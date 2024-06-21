using CoolWebApi.Models.Responses.Content;
using CoolWebApi.Utils.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace CoolWebApi.Services.AWS
{
    public interface IS3Service : IService
    {
        Task<Result<IActionResult>> UploadFileAsync(IFormFile file, string bucketName, string? prefix);

    }
}