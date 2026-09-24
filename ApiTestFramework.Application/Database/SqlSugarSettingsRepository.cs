using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.Domain.Entities;
using SqlSugar;

namespace ApiTestFramework.Application.Database;

/// <summary>
/// 全局设置数据库仓储：单行表（Id 恒为 1），先删后插实现 upsert
/// </summary>
public class SqlSugarSettingsRepository : IRepository<GlobalSettings>
{
    private readonly SqlSugarClient _db;
    private GlobalSettings? _cache;

    public SqlSugarSettingsRepository(SqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取全局设置（带内存缓存；无数据时返回默认实例）
    /// </summary>
    public async Task<GlobalSettings> GetAsync()
    {
        if (_cache != null)
        {
            return _cache;
        }

        _cache = await _db.Queryable<GlobalSettings>().FirstAsync() ?? new GlobalSettings();
        return _cache;
    }

    /// <summary>
    /// 保存全局设置：强制 Id=1，先删后插，成功后更新缓存
    /// </summary>
    public async Task SaveAsync(GlobalSettings entity)
    {
        entity.Id = 1;
        await _db.Deleteable<GlobalSettings>().ExecuteCommandAsync();
        await _db.Insertable(entity).ExecuteCommandAsync();
        _cache = entity;
    }

    /// <summary>
    /// 清空全局设置并重置缓存为默认实例
    /// </summary>
    public async Task ResetAsync()
    {
        await _db.Deleteable<GlobalSettings>().ExecuteCommandAsync();
        _cache = new GlobalSettings();
    }
}
