using CoolBlazor.Infrastructure.Extensions;
using CoolBlazor.Infrastructure.Managers.Content;
using CoolBlazor.Infrastructure.Models.Responses.Content;
using CoolBlazor.Infrastructure.Utils.Wrapper;

namespace CoolBlazor.Infrastructure.Managers.Dashboard
{
    public class DashboardManager : IDashboardManager
    {
        private readonly HttpClient _httpClient;

        public DashboardManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IResult<DashboardDataResponse>> GetDataAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DashboardEndpoints.GetData);
            return await response.ToResult<DashboardDataResponse>();

        }
    }
}