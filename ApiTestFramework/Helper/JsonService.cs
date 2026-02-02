using System.IO;
using System.Text.Json;

namespace ApiTestFramework.Helper;

public class JsonService
{

    SnowflakeIdGenerator snowflakeIdGenerator = new SnowflakeIdGenerator();

    public Dictionary<string, List<GenericRecord>> ParseDirectory(string filePath)
    {
        var result = new Dictionary<string, List<GenericRecord>>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return result;
        }

        var text = File.ReadAllText(filePath);

        using var doc = JsonDocument.Parse(text);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
        {
            return result;
        }

        foreach (var prop in root.EnumerateObject())
        {
            var list = new List<GenericRecord>();


            if (prop.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in prop.Value.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        list.Add(ConvertJsonObjectToRecord(item));
                    }
                }
            }
            else if (prop.Value.ValueKind == JsonValueKind.Object)
            {
                list.Add(ConvertJsonObjectToRecord(prop.Value));
            }

            result[prop.Name] = list;
        }

        return result;
    }

    private GenericRecord ConvertJsonObjectToRecord(JsonElement obj)
    {
        var record = new GenericRecord();

        foreach (var property in obj.EnumerateObject())
        {
            var value = ConvertJsonElementToValue(property.Value);
            record.Set(property.Name, value);
        }

        return record;
    }

    private object? ConvertJsonElementToValue(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
            case JsonValueKind.Array:
                return JsonSerializer.Serialize(element);
            case JsonValueKind.String:
                var jString = element.GetString();
                if (jString == null)
                {
                    return null;
                }
                if (jString == "#id#")
                {
                    return ConvertIdTemplet();
                }
                if (jString.Contains("#random#"))
                {
                    return jString.Replace("#random#", Guid.NewGuid().ToString("N"));
                }
                if (jString.Contains("#projectId#"))
                {
                    return jString.Replace("#projectId#", APPGloal.Dic["projectId"]);
                }
                if (jString.Contains("#tableId#"))
                {
                    return jString.Replace("#tableId#", APPGloal.Dic["tableId"]);
                }
                return jString;
            case JsonValueKind.Number:
                if (element.TryGetInt64(out var l))
                {
                    return l;
                }

                if (element.TryGetDouble(out var d))
                {
                    return d;
                }

                return element.GetRawText();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            default:
                return element.GetRawText();
        }
    }

    private long ConvertIdTemplet()
    {
        return snowflakeIdGenerator.NextId();
    }

}
