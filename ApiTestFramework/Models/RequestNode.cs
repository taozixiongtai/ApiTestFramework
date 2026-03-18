using ApiTestFramework.Infrastructure.Enum;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ApiTestFramework.Models;

public partial class RequestNode : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _parentId;

    [ObservableProperty]
    private TreeNodeTypeEnum _nodeType;

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isSelected;

    public virtual List<TreeNodeMenuItem> GetContextMenuItems()
    {
        return new List<TreeNodeMenuItem>
        {
            new() { Action = TreeNodeMenuActionEnum.Delete, Header = "删除", Icon = "🗑" }
        };
    }
}
