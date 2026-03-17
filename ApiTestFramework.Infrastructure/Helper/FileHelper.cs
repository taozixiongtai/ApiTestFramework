﻿﻿﻿using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Infrastructure.Exceptions;
using System.Text.Json;

namespace ApiTestFramework.Infrastructure.Helper
{
    public static class FileHelper
    {

        /// <summary>
        /// 读取指定路径下所有 JSON 文件
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <param name="includeSubDirectories">是否包含子目录</param>
        /// <returns>key=文件路径, value=json内容</returns>
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

        /// <summary>
        /// 解析 JSON 文件内容为表名和记录列表的映射
        /// </summary>
        /// <param name="jsonFilePath">JSON 文件路径</param>
        /// <returns>key=表名, value=记录列表</returns>
        public static Dictionary<string, List<DynamicJsonObject>> ParseJsonToTableRecords(string jsonFilePath)
        {
            if (string.IsNullOrWhiteSpace(jsonFilePath))
                throw new BusinessException("文件路径不能为空");

            if (!File.Exists(jsonFilePath))
                throw new BusinessException($"文件不存在: {jsonFilePath}");

            var jsonContent = File.ReadAllText(jsonFilePath);
            return ParseJsonContentToTableRecords(jsonContent);
        }

        /// <summary>
        /// 解析 JSON 内容为表名和记录列表的映射
        /// </summary>
        /// <param name="jsonContent">JSON 内容字符串</param>
        /// <returns>key=表名, value=记录列表</returns>
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
