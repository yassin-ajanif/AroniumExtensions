using System.Collections.Generic;
using System.Threading.Tasks;
using AroniumFactures.Data.Entities;

namespace AroniumFactures.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByNameAsync(string name);
    Task<Customer> CreateAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}
