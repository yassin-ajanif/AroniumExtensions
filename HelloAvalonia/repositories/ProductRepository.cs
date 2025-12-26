using System.Threading.Tasks;
using AroniumFactures.Data;
using Microsoft.EntityFrameworkCore;

namespace AroniumFactures.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetProductCountAsync()
    {
        return await _context.Products.CountAsync();
    }
}
