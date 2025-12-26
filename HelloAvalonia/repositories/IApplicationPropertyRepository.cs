using System.Collections.Generic;
using System.Threading.Tasks;
using HelloAvalonia.Data.Entities;

namespace HelloAvalonia.Repositories;

public interface IApplicationPropertyRepository
{
    Task<ApplicationProperty?> GetByNameAsync(string name);
    Task<string?> GetValueAsync(string name);
    Task<List<ApplicationProperty>> GetAllAsync();
}




























