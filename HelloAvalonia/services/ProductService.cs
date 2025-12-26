using System.Threading.Tasks;
using AroniumFactures.Repositories;

namespace AroniumFactures.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<int> GetProductCountAsync()
    {
        return await _productRepository.GetProductCountAsync();
    }
}
