using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Domain.Models;
using Mapster;
using SqlSugar;

namespace ApiTestFramework.Infrastructure.Database;

/// <summary>
/// 请求树数据库仓储：以扁平行存取 RequestTreeItemEntity，读取时组装为业务模型树、保存时全删全插
/// </summary>
public class SqlSugarTreeRepository : ITreeRepository
{
    private readonly SqlSugarClient _db;
    private List<RequestTreeNode>? _cache;

    public SqlSugarTreeRepository(SqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 读取全部扁平行并组装为业务模型树（带内存缓存；孤儿节点按根处理）
    /// </summary>
    public async Task<List<RequestTreeNode>> GetAsync()
    {
        if (_cache != null)
        {
            return _cache;
        }

        var rows = await _db.Queryable<RequestTreeItemEntity>()
                .OrderBy(r => r.SortOrder)
                .ToListAsync();

        var nodesById = rows.ToDictionary(r => r.Id, r => r.Adapt<RequestTreeNode>());
        var roots = new List<RequestTreeNode>();
        foreach (var row in rows)
        {
            // 父节点存在则挂到其 Children（行已按 SortOrder 排序，组内顺序保持），否则视为根节点（含孤儿节点）
            var node = nodesById[row.Id];
            if (row.ParentId != null && nodesById.TryGetValue(row.ParentId, out var parent) && parent != node)
            {
                parent.Children.Add(node);
            }
            else
            {
                roots.Add(node);
            }
        }

        _cache = roots;
        return _cache;
    }

    /// <summary>
    /// 保存请求树：展平为行后在事务内全删全插，成功后更新缓存
    /// </summary>
    public async Task SaveAsync(List<RequestTreeNode> tree)
    {
        var rows = new List<RequestTreeItemEntity>();
        Flatten(tree, null, rows);

        var result = await _db.Ado.UseTranAsync(async () =>
        {
            await _db.Deleteable<RequestTreeItemEntity>().ExecuteCommandAsync();
            await _db.Insertable(rows).ExecuteCommandAsync();
        });
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException($"保存请求树失败：{result.ErrorMessage}", result.ErrorException);
        }

        _cache = tree;
    }

    /// <summary>
    /// 清空请求树全部数据并重置缓存
    /// </summary>
    public async Task ResetAsync()
    {
        await _db.Deleteable<RequestTreeItemEntity>().ExecuteCommandAsync();
        _cache = new List<RequestTreeNode>();
    }

    /// <summary>
    /// 递归展平业务模型树节点为待落库的实体行（记录 ParentId 与同级 SortOrder，从 0 开始；不复用原对象以避免修改内存树）
    /// </summary>
    /// <param name="nodes">当前层节点列表</param>
    /// <param name="parentId">父节点 Id（根层为 null）</param>
    /// <param name="result">输出的行集合</param>
    private static void Flatten(IList<RequestTreeNode> nodes, string? parentId, List<RequestTreeItemEntity> result)
    {
        var order = 0;
        foreach (var node in nodes)
        {
            result.Add(new RequestTreeItemEntity
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
