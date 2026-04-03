namespace ApiTestFramework.Infrastructure.Domain;

/// <summary>
/// 种子数据项，用于持久化种子数据信息
/// </summary>
public class SeedDataItem
{
    /// <summary>
    /// 文件路径
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}
