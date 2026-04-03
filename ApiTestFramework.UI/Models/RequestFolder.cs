using ApiTestFramework.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ApiTestFramework.UI.Models;

public partial class RequestFolder : RequestNode
{
    public RequestFolder()
    {
        NodeType = TreeNodeTypeEnum.Folder;
    }

    [ObservableProperty]
    private ObservableCollection<RequestNode> _children = new();
}
