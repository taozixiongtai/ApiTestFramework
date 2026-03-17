using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Infrastructure.Enum;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ApiTestFramework.Models;

public partial class RequestFolder : RequestNode
{
    public RequestFolder()
    {
        NodeType = TreeNodeTypeEnum.Folder;
    }

    [ObservableProperty]
    private ObservableCollection<RequestNode> _children = new();
}

public partial class RequestItemNode : RequestNode
{
    public RequestItemNode()
    {
        NodeType = TreeNodeTypeEnum.Request;
    }

    [ObservableProperty]
    private RequestVerbEnum _requestVerb;

    [ObservableProperty]
    private string _path = string.Empty;

    [ObservableProperty]
    private string _body = string.Empty;

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _headers = new();
}
