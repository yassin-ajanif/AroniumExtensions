using System.Threading.Tasks;

namespace HelloAvalonia.Services;

public interface IProductService
{
    Task<int> GetProductCountAsync();
}
