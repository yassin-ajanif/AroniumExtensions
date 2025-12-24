using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloAvalonia.Data;
using HelloAvalonia.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelloAvalonia.Repositories;

public class ApplicationPropertyRepository : IApplicationPropertyRepository
{
    private readonly AppDbContext _context;

    public ApplicationPropertyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationProperty?> GetByNameAsync(string name)
    {
        return await _context.ApplicationProperties
            .FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<string?> GetValueAsync(string name)
    {
        var property = await GetByNameAsync(name);
        return property?.Value;
    }

    public async Task<List<ApplicationProperty>> GetAllAsync()
    {
        return await _context.ApplicationProperties.ToListAsync();
    }
}










