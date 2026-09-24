using SqlSugar;

namespace ApiTestFramework.Domain.Entities;

public class GlobalSettings
{
    /// <summary>固定主键（单行表，恒为 1）</summary>
    [SugarColumn(IsPrimaryKey = true)]
    public int Id { get; set; } = 1;

    public string Token { get; set; } = string.Empty;

    [SugarColumn(IsJson = true)]
    public Dictionary<string, string> Variables { get; set; } = [];

    [SugarColumn(IsJson = true)]
    public Dictionary<string, string> GlobalHeaders { get; set; } = [];

    public string BaseUrl { get; set; } = string.Empty;
}
