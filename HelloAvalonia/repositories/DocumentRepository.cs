using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloAvalonia.Data;
using HelloAvalonia.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelloAvalonia.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Document>> GetAllAsync()
    {
        return await _context.Documents
            .Include(d => d.DocumentItems)
                .ThenInclude(di => di.Product)
            .Include(d => d.Customer)
            .Include(d => d.DocumentType)
            .OrderByDescending(d => d.DateCreated)
            .ToListAsync();
    }

    public async Task<Document?> GetByIdAsync(int id)
    {
        return await _context.Documents
            .Include(d => d.DocumentItems)
                .ThenInclude(di => di.Product)
            .Include(d => d.Customer)
            .Include(d => d.DocumentType)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Document?> GetByNumberAsync(string number)
    {
        return await _context.Documents
            .Include(d => d.DocumentItems)
                .ThenInclude(di => di.Product)
            .Include(d => d.Customer)
            .Include(d => d.DocumentType)
            .FirstOrDefaultAsync(d => d.Number == number);
    }

    public async Task<List<Document>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Documents
            .Include(d => d.DocumentItems)
                .ThenInclude(di => di.Product)
            .Include(d => d.DocumentType)
            .Where(d => d.CustomerId == customerId)
            .OrderByDescending(d => d.DateCreated)
            .ToListAsync();
    }

    public async Task<Document> CreateAsync(Document document)
    {
        document.DateCreated = DateTime.Now;
        document.DateUpdated = DateTime.Now;
        
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        
        return document;
    }

    public async Task UpdateAsync(Document document)
    {
        document.DateUpdated = DateTime.Now;
        
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document != null)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetNextDocumentNumberAsync()
    {
        var lastDocument = await _context.Documents
            .OrderByDescending(d => d.Id)
            .FirstOrDefaultAsync();
        
        return (lastDocument?.Id ?? 0) + 1;
    }
}










