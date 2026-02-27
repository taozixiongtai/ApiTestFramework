using ApiTestFramework.Infrastructure.APP;
using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Service.Interface;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace ApiTestFramework.Service.Services;


public class DatabaseService : IDatabaseService
{
    private readonly AppOption _appOption;
    public DatabaseService(IOptions<AppOption> options)
    {
        _appOption = options.Value;
    }

    public void InsertData(string tableName, List<DynamicJsonObject> records)
    {
        if (records == null || records.Count == 0)
        {
            return;
        }

        using var connection = new MySqlConnection(_appOption.ConnectionString);  // 修改这里
        connection.Open();

        // 获取第一条记录的字段名
        var firstRecord = records.First();
        var properties = firstRecord.GetProperties();
        var columns = properties.Keys.ToList();

        // 创建参数化SQL
        var sql = GenerateInsertSql(tableName, columns);

        using var transaction = connection.BeginTransaction();
        try
        {
            foreach (var record in records)
            {
                using var command = new MySqlCommand(sql, connection, transaction);  // 修改这里
                                                                                     // 添加参数
                foreach (var column in columns)
                {
                    var value = record.GetValue(column) ?? DBNull.Value;
                    command.Parameters.AddWithValue($"@{column}", value);
                }

                command.ExecuteNonQuery();
            }

            transaction.Commit();
            Console.WriteLine($"成功插入 {records.Count} 条记录到表 {tableName}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"插入失败: {ex.Message}");
            throw;
        }
    }


    private static string GenerateInsertSql(string tableName, List<string> columns)
    {
        var columnNames = string.Join(", ", columns.Select(c => $"`{c}`"));  // MySQL使用反引号
        var parameterNames = string.Join(", ", columns.Select(c => $"@{c}"));

        return $"INSERT INTO `{tableName}` ({columnNames}) VALUES ({parameterNames})";  // 修改这里
    }

}