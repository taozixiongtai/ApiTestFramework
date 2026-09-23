using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Domain.Enums;
using ApiTestFramework.Infrastructure.Extensions;
using ApiTestFramework.UI.Mapper;
using ApiTestFramework.UI.Messages;
using ApiTestFramework.UI.Models;
using ApiTestFramework.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ApiTestFramework.UI.ViewModels;

public partial class RequestTreeViewModel : ObservableObject
{
    private readonly IRepository<List<RequestTreeItem>> _treeRepository;

    private Dictionary<TreeNodeMenuActionEnum, ICommand>? _actionCommands;

    [ObservableProperty]
    private ObservableCollection<RequestNode> _nodes = new();

    [ObservableProperty]
    private RequestNode? _selectedNode;

    [ObservableProperty]
    private ObservableCollection<TreeNodeMenuItem> _contextMenuItems = new();

    public RequestTreeViewModel(IRepository<List<RequestTreeItem>> treeRepository)
    {
        _treeRepository = treeRepository;
        LoadFromData();

        WeakReferenceMessenger.Default.Register<CreateRequestMessage>(this, OnCreateRequest);
        WeakReferenceMessenger.Default.Register<SaveDataMessage>(this, OnSaveData);
        WeakReferenceMessenger.Default.Register<SaveRecordingToTreeMessage>(this, OnSaveRecordingToTree);
    }

    private Dictionary<TreeNodeMenuActionEnum, ICommand> ActionCommands => _actionCommands ??= new Dictionary<TreeNodeMenuActionEnum, ICommand>
    {
        { TreeNodeMenuActionEnum.Delete, DeleteNodeCommand },
    };

    private async void LoadFromData()
    {
        var treeData = await _treeRepository.GetAsync();
        Nodes = DataMapper.ToViewModel(treeData);
    }

    public async Task SaveToDataAsync()
    {
        var treeData = DataMapper.ToDomain(Nodes);
        await _treeRepository.SaveAsync(treeData);
    }

    [RelayCommand]
    private async Task AddFolder()
    {
        var newFolder = new RequestFolder { Name = "新建文件夹" };

        if (SelectedNode == null)
        {
            Nodes.Add(newFolder);
        }
        else if (SelectedNode is RequestFolder folder)
        {
            folder.Children.Add(newFolder);
            folder.IsExpanded = true;
        }
        else if (SelectedNode is RequestItemNode)
        {
            if (FindParent(Nodes, SelectedNode) is RequestFolder parent)
            {
                var index = parent.Children.IndexOf(SelectedNode);
                parent.Children.Insert(index + 1, newFolder);
            }
            else
            {
                var index = Nodes.IndexOf(SelectedNode);
                Nodes.Insert(index + 1, newFolder);
            }
        }

        await SaveToDataAsync();
    }

    [RelayCommand]
    private async Task AddRequest()
    {
        var newRequest = new RequestItemNode { Name = "新建请求", RequestVerb = RequestVerbEnum.Get };

        if (SelectedNode == null)
        {
            Nodes.Add(newRequest);
        }
        else if (SelectedNode is RequestFolder folder)
        {
            folder.Children.Add(newRequest);
            folder.IsExpanded = true;
        }
        else if (SelectedNode is RequestItemNode)
        {
            if (FindParent(Nodes, SelectedNode) is RequestFolder parent)
            {
                var index = parent.Children.IndexOf(SelectedNode);
                parent.Children.Insert(index + 1, newRequest);
            }
            else
            {
                var index = Nodes.IndexOf(SelectedNode);
                Nodes.Insert(index + 1, newRequest);
            }
        }

        await SaveToDataAsync();
    }

    [RelayCommand]
    private async Task DeleteNode()
    {
        if (SelectedNode == null) return;

        var parent = FindParent(Nodes, SelectedNode);
        if (parent is RequestFolder parentFolder)
        {
            parentFolder.Children.Remove(SelectedNode);
        }
        else
        {
            Nodes.Remove(SelectedNode);
        }

        SelectedNode = null;
        await SaveToDataAsync();
    }

    private async void OnCreateRequest(object recipient, CreateRequestMessage message)
    {
        RequestNode? newNode = null;

        if (SelectedNode is RequestFolder folder)
        {
            var newRequest = new RequestItemNode { Name = "新建请求", RequestVerb = RequestVerbEnum.Get };
            folder.Children.Add(newRequest);
            folder.IsExpanded = true;
            newNode = newRequest;
        }
        else
        {
            var newRequest = new RequestItemNode { Name = "新建请求", RequestVerb = RequestVerbEnum.Get };
            Nodes.Add(newRequest);
            newNode = newRequest;
        }

        await SaveToDataAsync();
        WeakReferenceMessenger.Default.Send(new NodeSelectedMessage(newNode));
    }

    private async void OnSaveData(object recipient, SaveDataMessage message)
    {
        await SaveToDataAsync();
    }

    /// <summary>
    /// 接收录制保存消息：将捕获的请求按顺序转换为请求树中的文件夹与请求节点
    /// </summary>
    private async void OnSaveRecordingToTree(object recipient, SaveRecordingToTreeMessage message)
    {
        var session = message.Session;
        var folder = new RequestFolder { Name = session.Name, IsExpanded = true };

        foreach (var item in session.Requests.OrderBy(r => r.Order))
        {
            folder.Children.Add(ToRequestNode(item));
        }

        Nodes.Add(folder);
        await SaveToDataAsync();
    }

    /// <summary>
    /// 将录制的 HTTP 请求转换为请求树节点（方法/URL/请求头/请求体按录制结果填充）
    /// </summary>
    /// <param name="item">录制的 HTTP 请求</param>
    /// <returns>请求树节点</returns>
    private static RequestItemNode ToRequestNode(RecordedHttpRequest item)
    {
        var node = new RequestItemNode
        {
            Name = $"{item.Order}. {item.Method} {GetUrlPath(item.Url)}",
            RequestVerb = ToRequestVerb(item.Method),
            Path = item.Url,
            Body = item.Body
        };

        foreach (var header in item.Headers)
        {
            node.Headers.Add(new KeyValuePair<string, string>(header.Key, header.Value));
        }

        return node;
    }

    /// <summary>
    /// 将 HTTP 方法字符串解析为请求动词枚举（无法识别时默认 GET）
    /// </summary>
    private static RequestVerbEnum ToRequestVerb(string method) => method.ToUpperInvariant() switch
    {
        "GET" => RequestVerbEnum.Get,
        "POST" => RequestVerbEnum.Post,
        "PUT" => RequestVerbEnum.Put,
        "DELETE" => RequestVerbEnum.Delete,
        "PATCH" => RequestVerbEnum.Patch,
        _ => RequestVerbEnum.Get
    };

    /// <summary>
    /// 提取 URL 的路径部分用于节点命名（解析失败时返回原 URL）
    /// </summary>
    private static string GetUrlPath(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.AbsolutePath : url;
    }

    public void UpdateContextMenuItems()
    {
        ContextMenuItems.Clear();
        if (SelectedNode == null) return;

        foreach (var (action, description) in EnumExtension.GetAllDescriptions<TreeNodeMenuActionEnum>())
        {
            if (ActionCommands.TryGetValue(action, out var command))
            {
                ContextMenuItems.Add(new TreeNodeMenuItem
                {
                    Header = description,
                    Command = command
                });
            }
        }
    }

    private RequestNode? FindParent(ObservableCollection<RequestNode> nodes, RequestNode target)
    {
        foreach (var node in nodes)
        {
            var children = node switch
            {
                RequestFolder folder => folder.Children,
                _ => null
            };

            if (children == null) continue;

            if (children.Contains(target))
                return node;

            var found = FindParent(children, target);
            if (found != null)
                return found;
        }
        return null;
    }

    public void OnNodeSelected(RequestNode node)
    {
        SelectedNode = node;
        WeakReferenceMessenger.Default.Send(new NodeSelectedMessage(node));
    }

    public async Task UpdateNodeAsync(RequestNode node)
    {
        await SaveToDataAsync();
    }
}