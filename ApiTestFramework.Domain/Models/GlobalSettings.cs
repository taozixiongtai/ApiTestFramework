namespace ApiTestFramework.Domain.Models;

/// <summary>
/// 全局设置业务模型（无持久化细节），由仓储与持久化实体 GlobalSettingsEntity 互相转换
/// </summary>
public class GlobalSettings
{
    public string Token { get; set; } = string.Empty;

    public Dictionary<string, string> Variables { get; set; } = [];

    public Dictionary<string, string> GlobalHeaders { get; set; } = [];

    public string BaseUrl { get; set; } = string.Empty;
}
