using ApiTestFramework.Domain.Enums;

namespace ApiTestFramework.Domain.Models;

/// <summary>
/// 请求树节点业务模型（无持久化细节），由仓储与持久化实体 RequestTreeItemEntity 互相转换
/// </summary>
public class RequestTreeNode
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TreeNodeTypeEnum NodeType { get; set; } = TreeNodeTypeEnum.Folder;

    public bool IsExpanded { get; set; }

    public List<RequestTreeNode> Children { get; set; } = [];

    public RequestItem? RequestItem { get; set; }
}
