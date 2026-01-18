using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace AroniumFactures;

public static class DatabaseInitializer
{
    // Validates that the main database is reachable in read-only mode (no copy).
    public static Task<bool> InitializeDatabaseAsync(string mainDbPath)
    {
        try
        {
            Console.WriteLine($"Validating access to main database at: {mainDbPath}");
            
            if (!File.Exists(mainDbPath))
            {
                Console.WriteLine($"ERROR: Main database not found at: {mainDbPath}");
                return Task.FromResult(false);
            }

            var connectionString = $"Data Source={mainDbPath};Mode=ReadOnly;Cache=Shared;Default Timeout=3;";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT 1";
            cmd.ExecuteScalar();

            Console.WriteLine("Database validation succeeded (read-only).");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database initialization failed: {ex.Message}");
            return Task.FromResult(false);
        }
    }
}















































