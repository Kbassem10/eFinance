using System.Data;
using System.Data.Common;
using MySqlConnector;

namespace StudentRegistrationPortal.Api.Data;

/// <summary>
/// Factory interface for managing ADO.NET database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new, unopened MySQL connection instance.
    /// </summary>
    MySqlConnection CreateConnection();

    /// <summary>
    /// Creates and asynchronously opens a new MySQL connection instance.
    /// </summary>
    Task<MySqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
