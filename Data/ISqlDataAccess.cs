using System.Data;
using System.Data.Common;
using MySqlConnector;

namespace StudentRegistrationPortal.Api.Data;

/// <summary>
/// Core ADO.NET Data Access service providing safe, parameterized query and stored procedure execution.
/// </summary>
public interface ISqlDataAccess
{
    Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql,
        Func<MySqlDataReader, T> map,
        IEnumerable<MySqlParameter>? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);

    Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        Func<MySqlDataReader, T> map,
        IEnumerable<MySqlParameter>? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);

    Task<int> ExecuteNonQueryAsync(
        string sql,
        IEnumerable<MySqlParameter>? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);

    Task<T?> ExecuteScalarAsync<T>(
        string sql,
        IEnumerable<MySqlParameter>? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);

    Task<StoredProcedureResult> ExecuteStoredProcedureWithStatusAsync(
        string storedProcedureName,
        IEnumerable<MySqlParameter> parameters,
        string statusParamName = "p_ProcessingStatus",
        string messageParamName = "p_ProcessingMessage",
        string? idInOutParamName = null,
        CancellationToken cancellationToken = default);
}
