using ApiTestFramework.Domain.Enums;
using SqlSugar;

namespace ApiTestFramework.Domain.Entities;

public class RequestTreeItem
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    [SugarColumn(IsNullable = true)]
    public string? ParentId { get; set; }

    public TreeNodeTypeEnum NodeType { get; set; } = TreeNodeTypeEnum.Folder;

    public bool IsExpanded { get; set; }

    /// <summary>同级排序序号（从 0 开始）</summary>
    [SugarColumn(IsNullable = true)]
    public int SortOrder { get; set; }

    [SugarColumn(IsIgnore = true)]
    public List<RequestTreeItem> Children { get; set; } = [];

    [SugarColumn(IsJson = true, IsNullable = true)]
    public RequestItem? RequestItem { get; set; }
}
