using SqlSugar;

namespace ApiTestFramework.Domain.Entities;

/// <summary>
/// 全局设置持久化实体（单行表，Id 恒为 1），与业务模型 GlobalSettings 分离
/// </summary>
public class GlobalSettingsEntity
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
