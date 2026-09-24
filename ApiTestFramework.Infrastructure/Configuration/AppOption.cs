namespace ApiTestFramework.Infrastructure.Configuration;

public class AppOption
{
    public string? BaseUrl { get; set; }

    public string? LoginUrl { get; set; }

    public string? LoginPassword { get; set; }

    public string? LoginUserName { get; set; }

    public Dictionary<string, string>? RequestHeader { set; get; }

    /// <summary>
    /// 数据库配置节
    /// </summary>
    public DatabaseOption Database { get; set; } = new();
}

/// <summary>
/// 数据库连接配置（DbType 支持 SqlSugar 的类型名，如 Sqlite/MySql/SqlServer/PostgreSQL/Oracle）
/// </summary>
public class DatabaseOption
{
    /// <summary>数据库类型（默认 SQLite）</summary>
    public string DbType { get; set; } = "Sqlite";

    /// <summary>连接字符串</summary>
    public string ConnectionString { get; set; } = "Data Source=ApiTestFramework.db";
}
