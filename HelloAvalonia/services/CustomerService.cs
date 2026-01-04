using System.Collections.Generic;
using System.Threading.Tasks;
using AroniumFactures.Data.Entities;
using AroniumFactures.Repositories;

namespace AroniumFactures.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return await _customerRepository.GetAllAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        return await _customerRepository.GetByIdAsync(id);
    }

    public async Task<Customer?> GetCustomerByNameAsync(string name)
    {
        return await _customerRepository.GetByNameAsync(name);
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        return await _customerRepository.CreateAsync(customer);
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task DeleteCustomerAsync(int id)
    {
        await _customerRepository.DeleteAsync(id);
    }
}









































