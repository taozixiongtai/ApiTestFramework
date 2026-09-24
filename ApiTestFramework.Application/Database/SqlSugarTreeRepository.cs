using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Domain.Enums;
using SqlSugar;

namespace ApiTestFramework.Application.Database;

/// <summary>
/// 请求树数据库仓储：以扁平行存取 RequestTreeItem，读取时组装树、保存时全删全插
/// </summary>
public class SqlSugarTreeRepository : IRepository<List<RequestTreeItem>>
{
    private readonly SqlSugarClient _db;
    private List<RequestTreeItem>? _cache;

    public SqlSugarTreeRepository(SqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取全部扁平行并组装为树（带内存缓存；跳过遗留 Seed 节点，孤儿节点按根处理）
    /// </summary>
    public async Task<List<RequestTreeItem>> GetAsync()
    {
        if (_cache != null)
        {
            return _cache;
        }

        var rows = await _db.Queryable<RequestTreeItem>()
                .OrderBy(r => r.SortOrder)
                .ToListAsync();

        var byId = rows.ToDictionary(r => r.Id);
        var roots = new List<RequestTreeItem>();
        foreach (var row in rows)
        {
            // 父节点存在则挂到其 Children（行已按 SortOrder 排序，组内顺序保持），否则视为根节点（含孤儿节点）
            if (row.ParentId != null && byId.TryGetValue(row.ParentId, out var parent) && parent != row)
            {
                parent.Children.Add(row);
            }
            else
            {
                roots.Add(row);
            }
        }
         
        _cache = roots;
        return _cache;
    }

    /// <summary>
    /// 保存请求树：展平为行后在事务内全删全插，成功后更新缓存
    /// </summary>
    public async Task SaveAsync(List<RequestTreeItem> entity)
    {
        var rows = new List<RequestTreeItem>();
        Flatten(entity, null, rows);

        var result = await _db.Ado.UseTranAsync(async () =>
        {
            await _db.Deleteable<RequestTreeItem>().ExecuteCommandAsync();
            await _db.Insertable(rows).ExecuteCommandAsync();
        });
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException($"保存请求树失败：{result.ErrorMessage}", result.ErrorException);
        }

        _cache = entity;
    }

    /// <summary>
    /// 清空请求树全部数据并重置缓存
    /// </summary>
    public async Task ResetAsync()
    {
        await _db.Deleteable<RequestTreeItem>().ExecuteCommandAsync();
        _cache = new List<RequestTreeItem>();
    }

    /// <summary>
    /// 递归展平树节点为待落库的独立行对象（记录 ParentId 与同级 SortOrder，从 0 开始；跳过遗留 Seed 节点，不复用原对象以避免修改内存树）
    /// </summary>
    /// <param name="nodes">当前层节点列表</param>
    /// <param name="parentId">父节点 Id（根层为 null）</param>
    /// <param name="result">输出的行集合</param>
    private static void Flatten(IList<RequestTreeItem> nodes, string? parentId, List<RequestTreeItem> result)
    {
        var order = 0;
        foreach (var node in nodes)
        {
            result.Add(new RequestTreeItem
            {
                Id = node.Id,
                Name = node.Name,
                ParentId = parentId,
                NodeType = node.NodeType,
                IsExpanded = node.IsExpanded,
                SortOrder = order++,
                RequestItem = node.RequestItem
            });

            Flatten(node.Children, node.Id, result);
        }
    }
}
