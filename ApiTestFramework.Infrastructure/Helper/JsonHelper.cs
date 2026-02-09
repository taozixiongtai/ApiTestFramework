using ApiTestFramework.Infrastructure.APP;
using ApiTestFramework.Infrastructure.Domain;
using System.Text.Json;

namespace ApiTestFramework.Infrastructure.Helper;

internal class JsonHelper
{
    public Dictionary<string, List<DynamicJsonObject>> ParseDirectory(string jsonString)
    {
        var result = new Dictionary<string, List<DynamicJsonObject>>(StringComparer.OrdinalIgnoreCase);

        using var doc = JsonDocument.Parse(jsonString);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
        {
            return result;
        }

        foreach (var prop in root.EnumerateObject())
        {
            var list = new List<DynamicJsonObject>();


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

    private DynamicJsonObject ConvertJsonObjectToRecord(JsonElement obj)
    {
        var record = new DynamicJsonObject();

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
                    return 1;
                }
                if (jString.Contains("#random#"))
                {
                    return jString.Replace("#random#", Guid.NewGuid().ToString("N"));
                }
                if (jString.Contains("#projectId#"))
                {
                    return jString.Replace("#projectId#", APPGloal.RequestVariable["projectId"]);
                }
                if (jString.Contains("#tableId#"))
                {
                    return jString.Replace("#tableId#", APPGloal.RequestVariable["tableId"]);
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
}
