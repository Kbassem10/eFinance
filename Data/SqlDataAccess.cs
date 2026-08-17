using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace StudentRegistrationPortal.Api.Data;

/// <summary>
/// High-performance ADO.NET SQL Data Access Layer adhering to defensive programming and async patterns.
/// </summary>
public class SqlDataAccess : ISqlDataAccess
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<SqlDataAccess> _logger;

    public SqlDataAccess(IDbConnectionFactory connectionFactory, ILogger<SqlDataAccess> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql,
        Func<MySqlDataReader, T> map,
        IEnumerable<MySqlParameter>? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        ArgumentNullException.ThrowIfNull(map);

        var results = new List<T>();

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            await using var command = new MySqlCommand(sql, connection)
            {
                CommandType = commandType
            };

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param.Clone());
                }
            }

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(map(reader));
            }

            return results;
        }
        catch (MySqlException ex)
        {
            _logger.LogError(ex, "MySQL Error executing ADO.NET query: {Sql}", sql);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected Error executing ADO.NET query: {Sql}", sql);
            throw;
        }
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        Func<MySqlDataReader, T> map,
        IEnumerable<MySqlParameter>? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default)
    {
        var list = await QueryAsync(sql, map, parameters, commandType, cancellationToken);
        return list.Count > 0 ? list[0] : default;
    }

    public async Task<int> ExecuteNonQueryAsync(
        string sql,
        IEnumerable<MySqlParameter>? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            await using var command = new MySqlCommand(sql, connection)
            {
                CommandType = commandType
            };

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param.Clone());
                }
            }

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (MySqlException ex)
        {
            _logger.LogError(ex, "MySQL Error executing non-query: {Sql}", sql);
            throw;
        }
    }

    public async Task<T?> ExecuteScalarAsync<T>(
        string sql,
        IEnumerable<MySqlParameter>? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            await using var command = new MySqlCommand(sql, connection)
            {
                CommandType = commandType
            };

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param.Clone());
                }
            }

            var result = await command.ExecuteScalarAsync(cancellationToken);
            if (result == null || result is DBNull)
            {
                return default;
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }
        catch (MySqlException ex)
        {
            _logger.LogError(ex, "MySQL Error executing scalar: {Sql}", sql);
            throw;
        }
    }

    public async Task<StoredProcedureResult> ExecuteStoredProcedureWithStatusAsync(
        string storedProcedureName,
        IEnumerable<MySqlParameter> parameters,
        string statusParamName = "p_ProcessingStatus",
        string messageParamName = "p_ProcessingMessage",
        string? idInOutParamName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storedProcedureName);
        ArgumentNullException.ThrowIfNull(parameters);

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            await using var command = new MySqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var paramList = parameters.ToList();

            // Ensure output status parameter is registered
            MySqlParameter? statusParam = paramList.FirstOrDefault(p =>
                p.ParameterName.Equals(statusParamName, StringComparison.OrdinalIgnoreCase) ||
                p.ParameterName.Equals($"@{statusParamName}", StringComparison.OrdinalIgnoreCase));

            if (statusParam == null)
            {
                statusParam = new MySqlParameter(statusParamName, MySqlDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                paramList.Add(statusParam);
            }

            // Ensure output message parameter is registered
            MySqlParameter? messageParam = paramList.FirstOrDefault(p =>
                p.ParameterName.Equals(messageParamName, StringComparison.OrdinalIgnoreCase) ||
                p.ParameterName.Equals($"@{messageParamName}", StringComparison.OrdinalIgnoreCase));

            if (messageParam == null)
            {
                messageParam = new MySqlParameter(messageParamName, MySqlDbType.VarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };
                paramList.Add(messageParam);
            }

            foreach (var param in paramList)
            {
                command.Parameters.Add(param);
            }

            await command.ExecuteNonQueryAsync(cancellationToken);

            int status = statusParam.Value != null && statusParam.Value != DBNull.Value
                ? Convert.ToInt32(statusParam.Value)
                : 0;

            string message = messageParam.Value != null && messageParam.Value != DBNull.Value
                ? messageParam.Value.ToString() ?? string.Empty
                : string.Empty;

            int? affectedId = null;
            if (!string.IsNullOrWhiteSpace(idInOutParamName))
            {
                var idParam = command.Parameters.Cast<MySqlParameter>().FirstOrDefault(p =>
                    p.ParameterName.Equals(idInOutParamName, StringComparison.OrdinalIgnoreCase) ||
                    p.ParameterName.Equals($"@{idInOutParamName}", StringComparison.OrdinalIgnoreCase));

                if (idParam?.Value != null && idParam.Value != DBNull.Value)
                {
                    affectedId = Convert.ToInt32(idParam.Value);
                }
            }

            return new StoredProcedureResult
            {
                ProcessingStatus = status,
                ProcessingMessage = message,
                AffectedId = affectedId
            };
        }
        catch (MySqlException ex)
        {
            _logger.LogError(ex, "MySQL Error executing stored procedure: {Procedure}", storedProcedureName);
            return StoredProcedureResult.Failure($"Database Exception: {ex.Message}", 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected Exception executing stored procedure: {Procedure}", storedProcedureName);
            return StoredProcedureResult.Failure($"Unexpected Error: {ex.Message}", 0);
        }
    }
}
