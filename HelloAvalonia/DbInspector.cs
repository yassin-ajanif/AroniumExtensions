using System;
using System.Threading.Tasks;
using AroniumFactures.Data;
using Microsoft.EntityFrameworkCore;

namespace AroniumFactures;

public static class DbInspector
{
    public static async Task PrintAllApplicationPropertiesAsync()
    {
        try
        {
            var db = ServiceProvider.DbContext;
            var properties = await db.ApplicationProperties.ToListAsync();
            
            Console.WriteLine("=== APPLICATION PROPERTIES ===");
            Console.WriteLine($"Total: {properties.Count} properties");
            Console.WriteLine();
            
            foreach (var prop in properties)
            {
                Console.WriteLine($"Name: '{prop.Name}'");
                Console.WriteLine($"Value: '{prop.Value}'");
                Console.WriteLine("---");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}










