using System.Threading.Tasks;

namespace AroniumFactures.Services;

public interface IProductService
{
    Task<int> GetProductCountAsync();
}
