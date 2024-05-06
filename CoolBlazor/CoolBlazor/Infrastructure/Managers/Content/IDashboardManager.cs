using System.Threading.Tasks;
using CoolBlazor.Infrastructure.Models.Responses.Content;
using CoolBlazor.Infrastructure.Utils.Wrapper;

namespace CoolBlazor.Infrastructure.Managers.Content
{
    public interface IDashboardManager : IManager
    {
        Task<IResult<DashboardDataResponse>> GetDataAsync();
    }
}