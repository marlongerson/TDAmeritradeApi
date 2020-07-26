using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TDAmeritradeApi
{
    internal class MillisecondDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long milliseconds = reader.GetInt64();

            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            long milliseconds = value.ToUnixTimeMilliseconds();

            writer.WriteNumberValue(milliseconds);
        }
    }
}