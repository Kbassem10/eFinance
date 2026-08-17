using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentRegistrationPortal.Api.Converters;

/// Allows string properties to accept both JSON strings and raw JSON numbers (e.g. 30001011234567 or "30001011234567").
public class LenientStringJsonConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.TryGetInt64(out long intVal)
                ? intVal.ToString()
                : reader.GetDouble().ToString(),
            JsonTokenType.Null => null,
            _ => reader.GetString()
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
