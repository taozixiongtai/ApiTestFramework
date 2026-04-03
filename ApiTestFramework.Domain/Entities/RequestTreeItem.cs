using ApiTestFramework.Domain.Enums;

namespace ApiTestFramework.Domain.Entities;

public class RequestTreeItem
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? ParentId { get; set; }

    public TreeNodeTypeEnum NodeType { get; set; } = TreeNodeTypeEnum.Folder;

    public bool IsExpanded { get; set; }

    public List<RequestTreeItem> Children { get; set; } = [];

    public RequestItem? RequestItem { get; set; }

    public SeedDataItem? SeedDataItem { get; set; }
}
