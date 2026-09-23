using ApiTestFramework.UI.Controls;
using ApiTestFramework.Domain.Entities;
using ApiTestFramework.UI.Models;
using ApiTestFramework.UI.Messages;
using ApiTestFramework.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace ApiTestFramework.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private RequestTreeViewModel _treeViewModel;

    [ObservableProperty]
    private RequestDetailViewModel _detailViewModel;

    [ObservableProperty]
    private WebRecorderViewModel _webRecorderViewModel;

    [ObservableProperty]
    private ReplayViewModel _replayViewModel;

    [ObservableProperty]
    private UserControl _currentDetailView = new EmptyControl();

    /// <summary>
    /// 录制页当前显示的视图
    /// </summary>
    [ObservableProperty]
    private UserControl _currentRecordingDetailView = new EmptyControl();

    /// <summary>
    /// 主界面当前选中的 Tab 页索引（0=请求，1=录制）
    /// </summary>
    [ObservableProperty]
    private int _selectedTabIndex;

    /// <summary>
    /// Web 录制控件缓存，避免切换节点时重新初始化 WebView2 丢失浏览器状态
    /// </summary>
    private WebRecorderControl? _webRecorderControl;

    public MainViewModel(
        IHttpClientService httpClientService,
        IRepository<List<RequestTreeItem>> treeRepository,
        IRepository<RecordedSessionCollection> sessionRepository,
        WebRecorderViewModel webRecorderViewModel,
        ReplayViewModel replayViewModel)
    {
        TreeViewModel = new RequestTreeViewModel(treeRepository, sessionRepository);
        DetailViewModel = new RequestDetailViewModel(httpClientService);
        WebRecorderViewModel = webRecorderViewModel;
        ReplayViewModel = replayViewModel;

        WeakReferenceMessenger.Default.Register<NodeSelectedMessage>(this, OnNodeSelected);
    }

    /// <summary>
    /// 首次切换到录制页时默认选中 Web 录制入口，直接进入录制视图
    /// </summary>
    partial void OnSelectedTabIndexChanged(int value)
    {
        if (value == 1 && CurrentRecordingDetailView is EmptyControl &&
            TreeViewModel.RecordingNodes.OfType<WebRecorderNode>().FirstOrDefault() is { } recorderNode)
        {
            TreeViewModel.OnNodeSelected(recorderNode);
        }
    }

    private void OnNodeSelected(object recipient, NodeSelectedMessage message)
    {
        var node = message.Node;

        DetailViewModel.SyncToNode();

        if (node is RequestItemNode request)
        {
            SelectedTabIndex = 0;
            DetailViewModel.LoadRequest(request);
            var control = new RequestDetailControl { DataContext = DetailViewModel };
            CurrentDetailView = control;
        }
        else if (node is WebRecorderNode)
        {
            SelectedTabIndex = 1;
            _webRecorderControl ??= new WebRecorderControl { DataContext = WebRecorderViewModel };
            CurrentRecordingDetailView = _webRecorderControl;
        }
        else if (node is RecordingSessionNode sessionNode)
        {
            SelectedTabIndex = 1;
            ReplayViewModel.LoadSession(sessionNode);
            CurrentRecordingDetailView = new ReplayControl { DataContext = ReplayViewModel };
        }
        else
        {
            SelectedTabIndex = 0;
            DetailViewModel.Clear();
            CurrentDetailView = new EmptyControl();
        }
    }

    [RelayCommand]
    private async Task SaveRequest()
    {
        DetailViewModel.SyncToNode();
        WeakReferenceMessenger.Default.Send(new SaveDataMessage());
        MessageBox.Show("数据已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void AddRequestInFolder()
    {
        WeakReferenceMessenger.Default.Send(new CreateRequestMessage());
    }
}