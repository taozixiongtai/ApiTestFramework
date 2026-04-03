using ApiTestFramework.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ApiTestFramework.UI.Models;

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
}
