using System.Reflection;
using DbUp;
using Microsoft.Extensions.Logging;

namespace StudentRegistrationPortal.Infrastructure.Persistence.Migrations;

public interface IDatabaseMigrator
{
    bool MigrateDatabase(string connectionString);
}

public class DatabaseMigrator : IDatabaseMigrator
{
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(ILogger<DatabaseMigrator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool MigrateDatabase(string connectionString)
    {
        try
        {
            EnsureDatabase.For.MySqlDatabase(connectionString);
            var upgrader = DeployChanges.To
                .MySqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();
            if (result.Successful)
            {
                _logger.LogInformation("Database migrations applied successfully.");
                return true;
            }

            _logger.LogWarning("Database migrations reported failures: {Error}", result.Error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not apply database migrations on startup. Ensure MySQL is running.");
            return false;
        }
    }
}

