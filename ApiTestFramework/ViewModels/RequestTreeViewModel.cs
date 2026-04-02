using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Infrastructure.Enum;
using ApiTestFramework.Infrastructure.Extensions;
using ApiTestFramework.Mapper;
using ApiTestFramework.Models;
using ApiTestFramework.Service.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ApiTestFramework.ViewModels;

/// <summary>
/// 请求树视图模型，管理左侧树形结构的显示和操作
/// </summary>
/// <remarks>
/// <para>该类负责管理请求集合的树形结构，包括：</para>
/// <list type="bullet">
///   <item><description>文件夹和请求节点的增删操作</description></item>
///   <item><description>树节点选择的处理和事件通知</description></item>
///   <item><description>右键菜单的动态生成</description></item>
///   <item><description>树结构数据的持久化</description></item>
/// </list>
/// <para>使用 CommunityToolkit.Mvvm 的 [ObservableProperty] 和 [RelayCommand] 特性实现 MVVM 模式</para>
/// </remarks>
public partial class RequestTreeViewModel : ObservableObject
{
    /// <summary>
    /// 树结构数据仓储，用于持久化树数据到 JSON 文件
    /// </summary>
    private readonly IRepository<List<RequestTreeItem>> _treeRepository;

    /// <summary>
    /// 菜单操作命令映射字典，将枚举操作映射到对应的 ICommand
    /// </summary>
    /// <remarks>
    /// 使用延迟初始化，确保 [RelayCommand] 生成的命令已可用
    /// </remarks>
    private Dictionary<TreeNodeMenuActionEnum, ICommand>? _actionCommands;

    /// <summary>
    /// 树节点集合，绑定到 TreeView 的 ItemsSource
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<RequestNode> _nodes = new();

    /// <summary>
    /// 当前选中的树节点
    /// </summary>
    /// <remarks>
    /// 用于确定操作的目标节点，如添加子节点、删除节点等
    /// </remarks>
    [ObservableProperty]
    private RequestNode? _selectedNode;

    /// <summary>
    /// 右键菜单项集合，动态生成并绑定到 ContextMenu
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<TreeNodeMenuItem> _contextMenuItems = new();

    /// <summary>
    /// 节点选择事件，当选中节点时触发
    /// </summary>
    /// <remarks>
    /// 用于通知 MainViewModel 更新右侧详情面板
    /// </remarks>
    public event Action<RequestNode>? NodeSelected;

    /// <summary>
    /// 初始化 RequestTreeViewModel 的新实例
    /// </summary>
    /// <param name="treeRepository">树结构数据仓储</param>
    public RequestTreeViewModel(IRepository<List<RequestTreeItem>> treeRepository)
    {
        _treeRepository = treeRepository;
        LoadFromData();
    }

    /// <summary>
    /// 获取菜单操作命令映射字典
    /// </summary>
    /// <remarks>
    /// <para>使用延迟初始化模式，确保 [RelayCommand] 特性生成的命令已可用</para>
    /// <para>新增菜单功能时，只需在此字典中添加映射即可</para>
    /// </remarks>
    private Dictionary<TreeNodeMenuActionEnum, ICommand> ActionCommands => _actionCommands ??= new Dictionary<TreeNodeMenuActionEnum, ICommand>
    {
        { TreeNodeMenuActionEnum.Delete, DeleteNodeCommand },
    };

    /// <summary>
    /// 从数据源加载树结构数据
    /// </summary>
    /// <remarks>
    /// 通过 DataMapper 将领域模型转换为视图模型
    /// </remarks>
    private async void LoadFromData()
    {
        var treeData = await _treeRepository.GetAsync();
        Nodes = DataMapper.ToViewModel(treeData);
    }

    /// <summary>
    /// 将树结构数据异步保存到数据源
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 通过 DataMapper 将视图模型转换为领域模型后持久化
    /// </remarks>
    public async Task SaveToDataAsync()
    {
        var treeData = DataMapper.ToDomain(Nodes);
        await _treeRepository.SaveAsync(treeData);
    }

    /// <summary>
    /// 添加新文件夹节点
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// <para>添加位置规则：</para>
    /// <list type="bullet">
    ///   <item><description>未选中节点：添加到根级别</description></item>
    ///   <item><description>选中文件夹：添加为该文件夹的子节点，并展开文件夹</description></item>
    ///   <item><description>选中请求：添加到该请求同级位置的下一个</description></item>
    /// </list>
    /// </remarks>
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

    /// <summary>
    /// 添加新请求节点
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// <para>添加位置规则：</para>
    /// <list type="bullet">
    ///   <item><description>未选中节点：添加到根级别</description></item>
    ///   <item><description>选中文件夹：添加为该文件夹的子节点，并展开文件夹</description></item>
    ///   <item><description>选中请求：添加到该请求同级位置的下一个</description></item>
    /// </list>
    /// <para>新建请求默认使用 GET 方法</para>
    /// </remarks>
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

    /// <summary>
    /// 添加新种子数据节点
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// <para>添加位置规则：</para>
    /// <list type="bullet">
    ///   <item><description>未选中节点：添加到根级别</description></item>
    ///   <item><description>选中文件夹：添加为该文件夹的子节点，并展开文件夹</description></item>
    ///   <item><description>选中请求或种子数据：添加到该节点同级位置的下一个</description></item>
    /// </list>
    /// </remarks>
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

    /// <summary>
    /// 删除当前选中的节点
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// <para>删除规则：</para>
    /// <list type="bullet">
    ///   <item><description>如果节点在文件夹内，从文件夹的 Children 中移除</description></item>
    ///   <item><description>如果节点在根级别，从 Nodes 中移除</description></item>
    /// </list>
    /// <para>删除后会清空选中状态并保存数据</para>
    /// </remarks>
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

    /// <summary>
    /// 更新右键菜单项集合
    /// </summary>
    /// <remarks>
    /// <para>菜单项自动从 TreeNodeMenuActionEnum 枚举中生成：</para>
    /// <list type="number">
    ///   <item><description>遍历枚举的所有值</description></item>
    ///   <item><description>读取 [Description] 特性作为菜单显示文本</description></item>
    ///   <item><description>从 ActionCommands 字典获取对应的命令</description></item>
    ///   <item><description>创建 TreeNodeMenuItem 并添加到集合</description></item>
    /// </list>
    /// <para>新增菜单功能只需：添加枚举值 + 在 ActionCommands 中注册命令</para>
    /// </remarks>
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

    /// <summary>
    /// 递归查找指定节点的父文件夹
    /// </summary>
    /// <param name="nodes">要搜索的节点集合</param>
    /// <param name="target">目标节点</param>
    /// <returns>父文件夹，如果未找到则返回 null</returns>
    /// <remarks>
    /// 使用深度优先搜索遍历整个树结构
    /// </remarks>
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

    /// <summary>
    /// 处理节点选择事件
    /// </summary>
    /// <param name="node">被选中的节点</param>
    /// <remarks>
    /// 更新 SelectedNode 并触发 NodeSelected 事件，通知订阅者（如 MainViewModel）
    /// </remarks>
    public void OnNodeSelected(RequestNode node)
    {
        SelectedNode = node;
        NodeSelected?.Invoke(node);
    }

    /// <summary>
    /// 更新节点数据并保存
    /// </summary>
    /// <param name="node">要更新的节点</param>
    /// <returns>表示异步操作的任务</returns>
    public async Task UpdateNodeAsync(RequestNode node)
    {
        await SaveToDataAsync();
    }
}
