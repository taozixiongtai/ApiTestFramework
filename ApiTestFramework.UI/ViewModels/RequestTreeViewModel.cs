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
    private readonly IRepository<RecordedSessionCollection> _sessionRepository;

    private Dictionary<TreeNodeMenuActionEnum, ICommand>? _actionCommands;

    [ObservableProperty]
    private ObservableCollection<RequestNode> _nodes = new();

    [ObservableProperty]
    private RequestNode? _selectedNode;

    [ObservableProperty]
    private ObservableCollection<TreeNodeMenuItem> _contextMenuItems = new();

    public RequestTreeViewModel(IRepository<List<RequestTreeItem>> treeRepository, IRepository<RecordedSessionCollection> sessionRepository)
    {
        _treeRepository = treeRepository;
        _sessionRepository = sessionRepository;
        LoadFromData();

        WeakReferenceMessenger.Default.Register<CreateRequestMessage>(this, OnCreateRequest);
        WeakReferenceMessenger.Default.Register<CreateSeedDataMessage>(this, OnCreateSeedData);
        WeakReferenceMessenger.Default.Register<SaveDataMessage>(this, OnSaveData);
        WeakReferenceMessenger.Default.Register<RecordingSessionSavedMessage>(this, OnRecordingSessionSaved);
    }

    private Dictionary<TreeNodeMenuActionEnum, ICommand> ActionCommands => _actionCommands ??= new Dictionary<TreeNodeMenuActionEnum, ICommand>
    {
        { TreeNodeMenuActionEnum.Delete, DeleteNodeCommand },
    };

    private async void LoadFromData()
    {
        var treeData = await _treeRepository.GetAsync();
        Nodes = DataMapper.ToViewModel(treeData);
        await LoadSessionsAsync();
    }

    /// <summary>
    /// 从仓储加载录制会话并填充到 Web 录制节点下
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    private async Task LoadSessionsAsync()
    {
        var sessions = await _sessionRepository.GetAsync();

        var recorderNode = Nodes.OfType<WebRecorderNode>().FirstOrDefault();
        if (recorderNode == null)
        {
            recorderNode = new WebRecorderNode();
            Nodes.Add(recorderNode);
        }

        recorderNode.Children.Clear();
        foreach (var session in sessions)
        {
            recorderNode.Children.Add(new RecordingSessionNode { Session = session, Name = session.Name });
        }
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
            if (FindParent(Nodes, SelectedNode) is RequestFolder parent)
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

        // Web 录制入口节点不允许删除
        if (SelectedNode is WebRecorderNode)
        {
            return;
        }

        // 录制会话节点从会话仓储中删除
        if (SelectedNode is RecordingSessionNode sessionNode)
        {
            var sessions = await _sessionRepository.GetAsync();
            sessions.RemoveAll(s => s.Id == sessionNode.Session.Id);
            await _sessionRepository.SaveAsync(sessions);
            await LoadSessionsAsync();
            SelectedNode = null;
            return;
        }

        var parent = FindParent(Nodes, SelectedNode);
        if (parent is RequestFolder parentFolder)
        {
            parentFolder.Children.Remove(SelectedNode);
        }
        else if (parent is WebRecorderNode parentRecorder)
        {
            parentRecorder.Children.Remove(SelectedNode);
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

    private async void OnCreateSeedData(object recipient, CreateSeedDataMessage message)
    {
        RequestNode? newNode = null;

        if (SelectedNode is RequestFolder folder)
        {
            var newSeedData = new SeedDataNode { Name = "新建种子数据" };
            folder.Children.Add(newSeedData);
            folder.IsExpanded = true;
            newNode = newSeedData;
        }
        else
        {
            var newSeedData = new SeedDataNode { Name = "新建种子数据" };
            Nodes.Add(newSeedData);
            newNode = newSeedData;
        }

        await SaveToDataAsync();
        WeakReferenceMessenger.Default.Send(new NodeSelectedMessage(newNode));
    }

    private async void OnSaveData(object recipient, SaveDataMessage message)
    {
        await SaveToDataAsync();
    }

    private async void OnRecordingSessionSaved(object recipient, RecordingSessionSavedMessage message)
    {
        await LoadSessionsAsync();
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
                WebRecorderNode recorder => recorder.Children,
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