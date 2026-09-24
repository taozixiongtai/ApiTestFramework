using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace ApiTestFramework.Application.Database;

/// <summary>
/// SqlSugar 数据库上下文工厂：按配置的 DbType 创建连接并完成 CodeFirst 建表
/// </summary>
public class SqlSugarDbContext
{
    private readonly DatabaseOption _option;

    public SqlSugarDbContext(IOptions<AppOption> options)
    {
        _option = options.Value.Database ?? new DatabaseOption();
    }

    /// <summary>
    /// 创建 SqlSugarClient 并完成 CodeFirst 建表
    /// </summary>
    public SqlSugarClient CreateClient()
    {
        if (!Enum.TryParse<DbType>(_option.DbType, ignoreCase: true, out var dbType))
        {
            throw new InvalidOperationException($"不支持的数据库类型：{_option.DbType}（支持 Sqlite/MySql/SqlServer/PostgreSQL/Oracle 等 SqlSugar DbType）");
        }

        var db = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = _option.ConnectionString,
            DbType = dbType,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });

        db.CodeFirst.InitTables<RequestTreeItem>();
        db.CodeFirst.InitTables<GlobalSettings>();
        return db;
    }
}
