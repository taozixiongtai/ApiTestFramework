using ApiTestFramework.Domain.Models;

namespace ApiTestFramework.Infrastructure.Database;

/// <summary>
/// 请求树仓储接口：整棵树的读取与保存
/// </summary>
public interface ITreeRepository
{
    /// <summary>
    /// 读取整棵请求树
    /// </summary>
    /// <returns>根节点列表（按保存时的顺序）</returns>
    Task<List<RequestTreeNode>> GetAsync();

    /// <summary>
    /// 保存整棵请求树（全删全插）
    /// </summary>
    /// <param name="tree">根节点列表</param>
    Task SaveAsync(List<RequestTreeNode> tree);

    /// <summary>
    /// 清空请求树全部数据
    /// </summary>
    Task ResetAsync();
}
