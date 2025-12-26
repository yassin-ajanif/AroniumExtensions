using System.Threading.Tasks;

namespace AroniumFactures.Repositories;

public interface IProductRepository
{
    Task<int> GetProductCountAsync();
}
