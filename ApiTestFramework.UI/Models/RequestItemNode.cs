using ApiTestFramework.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ApiTestFramework.UI.Models;

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

    [ObservableProperty]
    private string _response = string.Empty;

    [ObservableProperty]
    private int _statusCode;

    [ObservableProperty]
    private double _responseTime;

    [ObservableProperty]
    private bool _hasResponse;
}
