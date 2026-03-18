using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Infrastructure.Enum;
using ApiTestFramework.Mapper;
using ApiTestFramework.Models;
using ApiTestFramework.Service.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ApiTestFramework.ViewModels;

public partial class RequestTreeViewModel : ObservableObject
{
    private readonly IRepository<List<RequestTreeItem>> _treeRepository;

    [ObservableProperty]
    private ObservableCollection<RequestNode> _nodes = new();

    [ObservableProperty]
    private RequestNode? _selectedNode;

    public event Action<RequestItemNode>? RequestSelected;

    public RequestTreeViewModel(IRepository<List<RequestTreeItem>> treeRepository)
    {
        _treeRepository = treeRepository;
        LoadFromData();
    }

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
            var parent = FindParent(Nodes, SelectedNode);
            if (parent != null)
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
            var parent = FindParent(Nodes, SelectedNode);
            if (parent != null)
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
        if (parent != null)
        {
            parent.Children.Remove(SelectedNode);
        }
        else
        {
            Nodes.Remove(SelectedNode);
        }

        SelectedNode = null;
        await SaveToDataAsync();
    }

    [RelayCommand]
    private async Task ExecuteMenuAction(object? parameter)
    {
        if (parameter is not TreeNodeMenuActionEnum action || SelectedNode == null)
            return;

        switch (action)
        {
            case TreeNodeMenuActionEnum.Delete:
                await DeleteNode();
                break;
            case TreeNodeMenuActionEnum.Rename:
            case TreeNodeMenuActionEnum.Duplicate:
            case TreeNodeMenuActionEnum.MoveUp:
            case TreeNodeMenuActionEnum.MoveDown:
                break;
        }
    }

    private RequestFolder? FindParent(ObservableCollection<RequestNode> nodes, RequestNode target)
    {
        foreach (var node in nodes)
        {
            if (node is RequestFolder folder)
            {
                if (folder.Children.Contains(target))
                    return folder;

                var found = FindParent(folder.Children, target);
                if (found != null)
                    return found;
            }
        }
        return null;
    }

    public void OnNodeSelected(RequestNode node)
    {
        SelectedNode = node;

        if (node is RequestItemNode requestNode)
        {
            RequestSelected?.Invoke(requestNode);
        }
    }

    public async Task UpdateNodeAsync(RequestNode node)
    {
        await SaveToDataAsync();
    }
}
