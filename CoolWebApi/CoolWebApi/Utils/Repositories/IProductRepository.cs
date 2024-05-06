using System.Threading.Tasks;

namespace CoolWebApi.Utils.Repositories
{
    public interface IProductRepository
    {
        Task<bool> IsBrandUsed(int brandId);
    }
}