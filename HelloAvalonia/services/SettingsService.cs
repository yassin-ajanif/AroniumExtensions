using System;
using System.IO;
using System.Threading.Tasks;
using HelloAvalonia.Data.Entities;
using HelloAvalonia.Repositories;

namespace HelloAvalonia.Services;

public class SettingsService : ISettingsService
{
    private readonly IApplicationPropertyRepository _propertyRepository;
    private const string DatabasePathKey = "Settings.MainDatabasePath";
    private const string DefaultDatabasePath = @"C:\ProgramData\Aronium\SimplePos\pos.db";

    public SettingsService(IApplicationPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<string?> GetMainDatabasePathAsync()
    {
        var path = await _propertyRepository.GetValueAsync(DatabasePathKey);
        
        // Return saved path or default
        return !string.IsNullOrEmpty(path) ? path : DefaultDatabasePath;
    }

    public async Task SaveMainDatabasePathAsync(string path)
    {
        var property = await _propertyRepository.GetByNameAsync(DatabasePathKey);
        
        if (property == null)
        {
            // Create new property
            property = new ApplicationProperty
            {
                Name = DatabasePathKey,
                Value = path
            };
            
            var context = ServiceProvider.DbContext;
            context.ApplicationProperties.Add(property);
            await context.SaveChangesAsync();
        }
        else
        {
            // Update existing
            property.Value = path;
            var context = ServiceProvider.DbContext;
            context.ApplicationProperties.Update(property);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasDatabasePathConfiguredAsync()
    {
        var path = await _propertyRepository.GetValueAsync(DatabasePathKey);
        return !string.IsNullOrEmpty(path) && File.Exists(path);
    }
}














