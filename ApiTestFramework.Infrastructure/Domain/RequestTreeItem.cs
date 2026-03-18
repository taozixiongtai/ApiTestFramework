using ApiTestFramework.Infrastructure.Enum;

namespace ApiTestFramework.Infrastructure.Domain;

/// <summary>
/// 请求树节点，用于持久化保存左侧树结构
/// </summary>
public class RequestTreeItem
{
    /// <summary>
    /// 节点ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 节点名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父节点ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 节点类型
    /// </summary>
    public TreeNodeTypeEnum NodeType { get; set; } = TreeNodeTypeEnum.Folder;

    /// <summary>
    /// 是否展开（仅当 NodeType 为 Folder 时有效）
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// 子节点（仅当 NodeType 为 Folder 时有效）
    /// </summary>
    public List<RequestTreeItem> Children { get; set; } = new();

    /// <summary>
    /// 请求项（仅当 NodeType 为 Request 时有效）
    /// </summary>
    public RequestItem? RequestItem { get; set; }
}
