using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CoolBlazor.Infrastructure.Utils.Wrapper;
using IResult = CoolBlazor.Infrastructure.Utils.Wrapper.IResult;

namespace CoolBlazor.Infrastructure.Extensions
{
    internal static class ResultExtensions
    {
        /// <summary>
        /// Process http response to return object
        /// </summary>
        /// <param name="response"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Object</returns>
        internal static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
        {
            try
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                return responseObject;
            }
            catch (Exception e)
            {
                Debug.WriteLine("ResultExtensions ToResult e = " + e);
            }
            return null;

        }

        internal static async Task<IResult> ToResult(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Result>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }

        internal static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseObject;
        }
    }
}