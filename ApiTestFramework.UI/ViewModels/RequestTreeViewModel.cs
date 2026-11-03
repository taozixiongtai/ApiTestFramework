using ApiTestFramework.Domain.Enums;
using ApiTestFramework.Infrastructure.Database;
using ApiTestFramework.Infrastructure.Extensions;
using ApiTestFramework.UI.Mapper;
using ApiTestFramework.UI.Messages;
using ApiTestFramework.UI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ApiTestFramework.UI.ViewModels;

public partial class RequestTreeViewModel : ObservableObject
{
    private readonly ITreeRepository _treeRepository;

    private Dictionary<TreeNodeMenuActionEnum, ICommand>? _actionCommands;

    [ObservableProperty]
    private ObservableCollection<RequestNode> _nodes = new();

    [ObservableProperty]
    private RequestNode? _selectedNode;

    [ObservableProperty]
    private ObservableCollection<TreeNodeMenuItem> _contextMenuItems = new();

    public RequestTreeViewModel(ITreeRepository treeRepository)
    {
        _treeRepository = treeRepository;
        LoadFromData();

        WeakReferenceMessenger.Default.Register<CreateRequestMessage>(this, OnCreateRequest);
        WeakReferenceMessenger.Default.Register<SaveDataMessage>(this, OnSaveData);
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