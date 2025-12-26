using System.Collections.Generic;
using System.Threading.Tasks;
using HelloAvalonia.Data.Entities;

namespace HelloAvalonia.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(int id);
    Task<Customer?> GetCustomerByNameAsync(string name);
    Task<Customer> CreateCustomerAsync(Customer customer);
    Task UpdateCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(int id);
}




























