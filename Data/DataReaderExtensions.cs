using System.Data;
using System.Data.Common;

namespace StudentRegistrationPortal.Api.Data;

/// <summary>
/// Defensive data access extensions for ADO.NET DbDataReader and IDataReader.
/// Prevents null pointer exceptions and DBNull casting issues with safe fallback defaults.
/// </summary>
public static class DataReaderExtensions
{
    public static bool HasColumn(this IDataReader reader, string columnName)
    {
        if (reader == null || string.IsNullOrWhiteSpace(columnName))
            return false;

        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static string GetSafeString(this IDataReader reader, string columnName, string fallback = "")
    {
        if (reader == null || !reader.HasColumn(columnName)) return fallback;
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? fallback : (reader.GetValue(ordinal)?.ToString() ?? fallback);
    }

    public static string? GetSafeNullableString(this IDataReader reader, string columnName)
    {
        if (reader == null || !reader.HasColumn(columnName)) return null;
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal)?.ToString();
    }

    public static int GetSafeInt32(this IDataReader reader, string columnName, int fallback = 0)
    {
        if (reader == null || !reader.HasColumn(columnName)) return fallback;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return fallback;

        object val = reader.GetValue(ordinal);
        return Convert.ToInt32(val);
    }

    public static int? GetSafeNullableInt32(this IDataReader reader, string columnName)
    {
        if (reader == null || !reader.HasColumn(columnName)) return null;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return null;

        object val = reader.GetValue(ordinal);
        return Convert.ToInt32(val);
    }

    public static long GetSafeInt64(this IDataReader reader, string columnName, long fallback = 0L)
    {
        if (reader == null || !reader.HasColumn(columnName)) return fallback;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return fallback;

        return Convert.ToInt64(reader.GetValue(ordinal));
    }

    public static decimal GetSafeDecimal(this IDataReader reader, string columnName, decimal fallback = 0m)
    {
        if (reader == null || !reader.HasColumn(columnName)) return fallback;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return fallback;

        return Convert.ToDecimal(reader.GetValue(ordinal));
    }

    public static decimal? GetSafeNullableDecimal(this IDataReader reader, string columnName)
    {
        if (reader == null || !reader.HasColumn(columnName)) return null;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return null;

        return Convert.ToDecimal(reader.GetValue(ordinal));
    }

    public static bool GetSafeBool(this IDataReader reader, string columnName, bool fallback = false)
    {
        if (reader == null || !reader.HasColumn(columnName)) return fallback;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return fallback;

        return Convert.ToBoolean(reader.GetValue(ordinal));
    }

    public static DateTime GetSafeDateTime(this IDataReader reader, string columnName, DateTime fallback = default)
    {
        if (reader == null || !reader.HasColumn(columnName)) return fallback;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return fallback;

        return Convert.ToDateTime(reader.GetValue(ordinal));
    }

    public static DateTime? GetSafeNullableDateTime(this IDataReader reader, string columnName)
    {
        if (reader == null || !reader.HasColumn(columnName)) return null;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return null;

        return Convert.ToDateTime(reader.GetValue(ordinal));
    }

    public static DateOnly GetSafeDateOnly(this IDataReader reader, string columnName, DateOnly fallback = default)
    {
        if (reader == null || !reader.HasColumn(columnName)) return fallback;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return fallback;

        object val = reader.GetValue(ordinal);
        if (val is DateOnly d) return d;
        if (val is DateTime dt) return DateOnly.FromDateTime(dt);
        if (DateOnly.TryParse(val?.ToString(), out var parsed)) return parsed;

        return fallback;
    }

    public static DateOnly? GetSafeNullableDateOnly(this IDataReader reader, string columnName)
    {
        if (reader == null || !reader.HasColumn(columnName)) return null;
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal)) return null;

        object val = reader.GetValue(ordinal);
        if (val is DateOnly d) return d;
        if (val is DateTime dt) return DateOnly.FromDateTime(dt);
        if (DateOnly.TryParse(val?.ToString(), out var parsed)) return parsed;

        return null;
    }
}
