using System.Reflection;
using DbUp;
using Microsoft.Extensions.Logging;

namespace StudentRegistrationPortal.Api.Data;

/// <summary>
/// Handles automated, version-controlled database migrations (similar to Django migrations) using DbUp.
/// Migrations are embedded SQL files executed in sequential order and tracked via the 'schemaversions' table.
/// </summary>
public static class DatabaseMigrator
{
    public static void ApplyMigrations(string connectionString, ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Database connection string cannot be null or empty.", nameof(connectionString));
        }

        logger?.LogInformation("Starting database migration process...");

        // Ensure target database exists
        EnsureDatabase.For.MySqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .MySqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            logger?.LogError(result.Error, "Database migration failed: {Error}", result.Error.Message);
            throw new InvalidOperationException($"Database migration failed: {result.Error.Message}", result.Error);
        }

        logger?.LogInformation("Database migration completed successfully. All pending scripts applied.");
    }
}
