
using CoolWebApi.Utils.Entities.Catalog;
using CoolWebApi.Utils.Repositories;

namespace CoolWebApi.Utils.Repositories.impl
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IRepositoryAsync<Brand, int> _repository;

        public BrandRepository(IRepositoryAsync<Brand, int> repository)
        {
            _repository = repository;
        }
    }
}