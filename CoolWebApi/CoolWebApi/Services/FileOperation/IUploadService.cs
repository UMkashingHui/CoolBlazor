using CoolWebApi.Models.Requests.File;
using CoolWebApi.Utils.Wrapper;

namespace CoolWebApi.Services.FileOperation
{
    public interface IUploadService
    {
        Task<string> UploadAsync(UploadRequest request);
    }
}