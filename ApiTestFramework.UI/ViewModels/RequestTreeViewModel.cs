using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Domain.Enums;
using ApiTestFramework.Infrastructure.Extensions;
using ApiTestFramework.UI.Mapper;
using ApiTestFramework.UI.Models;
using ApiTestFramework.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public event Action<RequestNode>? NodeSelected;

    public RequestTreeViewModel(IRepository<List<RequestTreeItem>> treeRepository)
    {
        _treeRepository = treeRepository;
        LoadFromData();
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
    private async Task AddSeedData()
    {
        var newSeedData = new SeedDataNode { Name = "新建种子数据" };

        if (SelectedNode == null)
        {
            Nodes.Add(newSeedData);
        }
        else if (SelectedNode is RequestFolder folder)
        {
            folder.Children.Add(newSeedData);
            folder.IsExpanded = true;
        }
        else if (SelectedNode is RequestItemNode or SeedDataNode)
        {
            var parent = FindParent(Nodes, SelectedNode);
            if (parent != null)
            {
                var index = parent.Children.IndexOf(SelectedNode);
                parent.Children.Insert(index + 1, newSeedData);
            }
            else
            {
                var index = Nodes.IndexOf(SelectedNode);
                Nodes.Insert(index + 1, newSeedData);
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
        NodeSelected?.Invoke(node);
    }

    public async Task UpdateNodeAsync(RequestNode node)
    {
        await SaveToDataAsync();
    }
}
