using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Models;
using ApiTestFramework.Service.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace ApiTestFramework.ViewModels;

/// <summary>
/// 主视图模型，作为应用程序的核心协调者
/// </summary>
/// <remarks>
/// <para>该类负责协调各个子 ViewModel 之间的交互，包括：</para>
/// <list type="bullet">
///   <item><description>RequestTreeViewModel - 管理左侧请求树结构</description></item>
///   <item><description>RequestDetailViewModel - 管理右侧请求详情面板</description></item>
///   <item><description>SettingsViewModel - 管理全局设置</description></item>
/// </list>
/// <para>采用 MVVM 模式，使用 CommunityToolkit.Mvvm 实现属性通知和命令绑定</para>
/// </remarks>
public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// 请求树视图模型，管理左侧树形结构的显示和操作
    /// </summary>
    [ObservableProperty]
    private RequestTreeViewModel _treeViewModel;

    /// <summary>
    /// 请求详情视图模型，管理右侧请求详情的显示和编辑
    /// </summary>
    [ObservableProperty]
    private RequestDetailViewModel _detailViewModel;

    /// <summary>
    /// 设置视图模型，管理全局配置项
    /// </summary>
    [ObservableProperty]
    private SettingsViewModel _settingsViewModel;

    /// <summary>
    /// 指示设置面板是否处于打开状态
    /// </summary>
    [ObservableProperty]
    private bool _isSettingsOpen;

    /// <summary>
    /// 初始化 MainViewModel 的新实例
    /// </summary>
    /// <param name="settingsRepository">全局设置数据仓储，用于持久化配置信息</param>
    /// <param name="treeRepository">请求树数据仓储，用于持久化树结构数据</param>
    /// <remarks>
    /// 构造函数会初始化所有子 ViewModel，并订阅树节点选择事件以实现联动
    /// </remarks>
    public MainViewModel(
        IRepository<GlobalSettings> settingsRepository,
        IRepository<List<RequestTreeItem>> treeRepository)
    {
        TreeViewModel = new RequestTreeViewModel(treeRepository);
        DetailViewModel = new RequestDetailViewModel();
        SettingsViewModel = new SettingsViewModel(settingsRepository);

        TreeViewModel.NodeSelected += OnNodeSelected;
    }

    /// <summary>
    /// 处理树节点选择事件，实现树与详情面板的联动
    /// </summary>
    /// <param name="node">被选中的树节点</param>
    /// <remarks>
    /// <para>处理逻辑：</para>
    /// <list type="number">
    ///   <item><description>先将当前详情面板的数据同步回原节点（保存未保存的修改）</description></item>
    ///   <item><description>如果选中的是请求节点，加载其详情到右侧面板</description></item>
    ///   <item><description>如果选中的是文件夹节点，清空右侧面板</description></item>
    /// </list>
    /// </remarks>
    private void OnNodeSelected(RequestNode node)
    {
        DetailViewModel.SyncToNode();

        if (node is RequestItemNode request)
        {
            DetailViewModel.LoadRequest(request);
        }
        else
        {
            DetailViewModel.Clear();
        }
    }

    /// <summary>
    /// 打开设置面板
    /// </summary>
    [RelayCommand]
    private void OpenSettings()
    {
        IsSettingsOpen = true;
    }

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    [RelayCommand]
    private void CloseSettings()
    {
        IsSettingsOpen = false;
    }

    /// <summary>
    /// 保存全局设置
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 保存后会自动关闭设置面板并显示成功提示
    /// </remarks>
    [RelayCommand]
    private async Task SaveSettings()
    {
        await SettingsViewModel.SaveAsync();
        IsSettingsOpen = false;
        MessageBox.Show("设置已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// 保存当前请求的修改
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// <para>保存流程：</para>
    /// <list type="number">
    ///   <item><description>将详情面板的数据同步到对应的请求节点</description></item>
    ///   <item><description>将整个树结构持久化到 JSON 文件</description></item>
    ///   <item><description>显示保存成功提示</description></item>
    /// </list>
    /// </remarks>
    [RelayCommand]
    private async Task SaveRequest()
    {
        DetailViewModel.SyncToNode();
        await TreeViewModel.SaveToDataAsync();
        MessageBox.Show("请求已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
