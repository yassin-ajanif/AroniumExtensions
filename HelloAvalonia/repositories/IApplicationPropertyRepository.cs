using System.Collections.Generic;
using System.Threading.Tasks;
using AroniumFactures.Data.Entities;

namespace AroniumFactures.Repositories;

public interface IApplicationPropertyRepository
{
    Task<ApplicationProperty?> GetByNameAsync(string name);
    Task<string?> GetValueAsync(string name);
    Task<List<ApplicationProperty>> GetAllAsync();
}







































