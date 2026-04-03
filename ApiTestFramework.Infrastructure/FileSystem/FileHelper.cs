using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Infrastructure.Exceptions;
using System.Text.Json;

namespace ApiTestFramework.Infrastructure.FileSystem
{
    public static class FileHelper
    {

        public static async Task<Dictionary<string, string>> ReadAllJsonFiles(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new BusinessException("目录不能为空");

            if (!Directory.Exists(path))
                throw new BusinessException($"目录不存在: {path}");

            var result = new Dictionary<string, string>();

            var files = Directory.GetFiles(path, "*.json");

            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file);
                result[file] = content;
            }

            return result;
        }

        public static Dictionary<string, List<DynamicJsonObject>> ParseJsonToTableRecords(string jsonFilePath)
        {
            if (string.IsNullOrWhiteSpace(jsonFilePath))
                throw new BusinessException("文件路径不能为空");

            if (!File.Exists(jsonFilePath))
                throw new BusinessException($"文件不存在: {jsonFilePath}");

            var jsonContent = File.ReadAllText(jsonFilePath);
            return ParseJsonContentToTableRecords(jsonContent);
        }

        public static Dictionary<string, List<DynamicJsonObject>> ParseJsonContentToTableRecords(string jsonContent)
        {
            var result = new Dictionary<string, List<DynamicJsonObject>>();

            using var jsonDoc = JsonDocument.Parse(jsonContent);

            foreach (var tableElement in jsonDoc.RootElement.EnumerateObject())
            {
                var tableName = tableElement.Name;
                var records = new List<DynamicJsonObject>();

                foreach (var item in tableElement.Value.EnumerateArray())
                {
                    var record = new DynamicJsonObject();
                    foreach (var prop in item.EnumerateObject())
                    {
                        object value = prop.Value.ValueKind switch
                        {
                            JsonValueKind.String => prop.Value.GetString() ?? "",
                            JsonValueKind.Number => prop.Value.GetDouble(),
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.Null => "",
                            _ => prop.Value.ToString()
                        };
                        record.Set(prop.Name, value);
                    }
                    records.Add(record);
                }

                result[tableName] = records;
            }

            return result;
        }
    }
}
