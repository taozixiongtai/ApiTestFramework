using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Domain.Models;
using Mapster;
using SqlSugar;

namespace ApiTestFramework.Infrastructure.Database;

/// <summary>
/// 全局设置数据库仓储：单行表（Id 恒为 1），事务内先删后插实现 upsert
/// </summary>
public class SqlSugarSettingsRepository : ISettingsRepository
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

        var entity = await _db.Queryable<GlobalSettingsEntity>().FirstAsync();
        _cache = entity?.Adapt<GlobalSettings>() ?? new GlobalSettings();
        return _cache;
    }

    /// <summary>
    /// 保存全局设置：强制 Id=1，事务内先删后插，成功后更新缓存
    /// </summary>
    public async Task SaveAsync(GlobalSettings settings)
    {
        var entity = settings.Adapt<GlobalSettingsEntity>();
        entity.Id = 1;

        var result = await _db.Ado.UseTranAsync(async () =>
        {
            await _db.Deleteable<GlobalSettingsEntity>().ExecuteCommandAsync();
            await _db.Insertable(entity).ExecuteCommandAsync();
        });
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException($"保存全局设置失败：{result.ErrorMessage}", result.ErrorException);
        }

        _cache = settings;
    }

    /// <summary>
    /// 清空全局设置并重置缓存为默认实例
    /// </summary>
    public async Task ResetAsync()
    {
        await _db.Deleteable<GlobalSettingsEntity>().ExecuteCommandAsync();
        _cache = new GlobalSettings();
    }
}
