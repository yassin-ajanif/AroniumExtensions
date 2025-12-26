using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AroniumFactures.Data;
using AroniumFactures.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AroniumFactures.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Where(c => c.IsCustomer == 1)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<Customer?> GetByNameAsync(string name)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        customer.DateCreated = DateTime.Now;
        customer.DateUpdated = DateTime.Now;
        customer.IsCustomer = 1;
        customer.IsEnabled = 1;
        
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        
        return customer;
    }

    public async Task UpdateAsync(Customer customer)
    {
        customer.DateUpdated = DateTime.Now;
        
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}





























