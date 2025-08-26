using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeneralPurpose.Api.Converters;

public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawValue = reader.GetString();
        if (string.IsNullOrEmpty(rawValue)) return DateTime.MinValue;
        var result = DateTime.Parse(rawValue);
        return result.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(result, DateTimeKind.Local).ToUniversalTime() : result;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var convertedValue = value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime()
            : value.ToUniversalTime();
        var jsonDateTimeFormat =
            convertedValue.ToString("yyyy-MM-dd'T'HH:mm:ssK", System.Globalization.CultureInfo.InvariantCulture);
        writer.WriteStringValue(jsonDateTimeFormat);
    }
}