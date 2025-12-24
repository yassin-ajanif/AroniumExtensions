using System.Threading.Tasks;

namespace HelloAvalonia.Repositories;

public interface IProductRepository
{
    Task<int> GetProductCountAsync();
}
