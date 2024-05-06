using CoolWebApi.Models.Responses.Content;
using CoolWebApi.Utils.Wrapper;

namespace CoolWebApi.Services.Content
{
    public interface IDashBoardService : IService
    {
        Task<Result<DashboardDataResponse>> GetDashBoardData();

    }
}