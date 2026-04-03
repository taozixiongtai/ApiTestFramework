using ApiTestFramework.Infrastructure.Configuration;
using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Application.Interfaces;
using Microsoft.Extensions.Options;
using SqlSugar;
using System.Data;

namespace ApiTestFramework.Application.Services;

public class DatabaseService : IDatabaseService
{
    private readonly SqlSugarClient _db;

    public DatabaseService(IOptions<AppOption> options)
    {
        var appOption = options.Value;

        var dbType = Enum.TryParse<SqlSugar.DbType>(appOption.DbType, ignoreCase: true, out var parsedDbType)
            ? parsedDbType
            : SqlSugar.DbType.MySql;

        _db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = appOption.ConnectionString,
            DbType = dbType,
            IsAutoCloseConnection = appOption.IsAutoCloseConnection
        });
    }

    public void InsertData(string tableName, List<DynamicJsonObject> records)
    {
        if (records == null || records.Count == 0)
        {
            return;
        }

        var firstRecord = records.First();
        var properties = firstRecord.GetProperties();

        var dataTable = new DataTable();
        foreach (var column in properties.Keys)
        {
            dataTable.Columns.Add(column);
        }

        foreach (var record in records)
        {
            var row = dataTable.NewRow();
            foreach (var column in properties.Keys)
            {
                var value = record.GetValue(column);
                row[column] = value ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);
        }

        _db.Fastest<DataTable>().BulkCopy(tableName, dataTable);
    }
}
