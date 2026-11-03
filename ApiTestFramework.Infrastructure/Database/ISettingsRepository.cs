using ApiTestFramework.Domain.Models;

namespace ApiTestFramework.Infrastructure.Database;

/// <summary>
/// 全局设置仓储接口：单行设置的读取与保存
/// </summary>
public interface ISettingsRepository
{
    /// <summary>
    /// 读取全局设置（无数据时返回默认实例）
    /// </summary>
    Task<GlobalSettings> GetAsync();

    /// <summary>
    /// 保存全局设置（单行 Upsert）
    /// </summary>
    /// <param name="settings">全局设置</param>
    Task SaveAsync(GlobalSettings settings);

    /// <summary>
    /// 清空全局设置
    /// </summary>
    Task ResetAsync();
}
