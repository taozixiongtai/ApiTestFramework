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
    private SeedDataDetailViewModel _seedDataDetailViewModel;

    [ObservableProperty]
    private WebRecorderViewModel _webRecorderViewModel;

    [ObservableProperty]
    private ReplayViewModel _replayViewModel;

    [ObservableProperty]
    private UserControl _currentDetailView = new EmptyControl();

    /// <summary>
    /// Web 录制控件缓存，避免切换节点时重新初始化 WebView2 丢失浏览器状态
    /// </summary>
    private WebRecorderControl? _webRecorderControl;

    public MainViewModel(
        IHttpClientService httpClientService,
        IRepository<List<RequestTreeItem>> treeRepository,
        IRepository<RecordedSessionCollection> sessionRepository,
        SeedDataDetailViewModel seedDataDetailViewModel,
        WebRecorderViewModel webRecorderViewModel,
        ReplayViewModel replayViewModel)
    {
        TreeViewModel = new RequestTreeViewModel(treeRepository, sessionRepository);
        DetailViewModel = new RequestDetailViewModel(httpClientService);
        SeedDataDetailViewModel = seedDataDetailViewModel;
        WebRecorderViewModel = webRecorderViewModel;
        ReplayViewModel = replayViewModel;

        WeakReferenceMessenger.Default.Register<NodeSelectedMessage>(this, OnNodeSelected);
    }

    private void OnNodeSelected(object recipient, NodeSelectedMessage message)
    {
        var node = message.Node;

        DetailViewModel.SyncToNode();
        SeedDataDetailViewModel.SyncToNode();

        if (node is RequestItemNode request)
        {
            DetailViewModel.LoadRequest(request);
            var control = new RequestDetailControl { DataContext = DetailViewModel };
            CurrentDetailView = control;
        }
        else if (node is SeedDataNode seedData)
        {
            seedData.CheckFileExists();
            SeedDataDetailViewModel.LoadSeedData(seedData);
            var control = new SeedDataDetailControl { DataContext = SeedDataDetailViewModel };
            CurrentDetailView = control;
        }
        else if (node is WebRecorderNode)
        {
            _webRecorderControl ??= new WebRecorderControl { DataContext = WebRecorderViewModel };
            CurrentDetailView = _webRecorderControl;
        }
        else if (node is RecordingSessionNode sessionNode)
        {
            ReplayViewModel.LoadSession(sessionNode);
            CurrentDetailView = new ReplayControl { DataContext = ReplayViewModel };
        }
        else
        {
            DetailViewModel.Clear();
            SeedDataDetailViewModel.Clear();
            CurrentDetailView = new EmptyControl();
        }
    }

    [RelayCommand]
    private async Task SaveRequest()
    {
        DetailViewModel.SyncToNode();
        SeedDataDetailViewModel.SyncToNode();
        WeakReferenceMessenger.Default.Send(new SaveDataMessage());
        MessageBox.Show("数据已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void AddRequestInFolder()
    {
        WeakReferenceMessenger.Default.Send(new CreateRequestMessage());
    }

    [RelayCommand]
    private void AddSeedDataInFolder()
    {
        WeakReferenceMessenger.Default.Send(new CreateSeedDataMessage());
    }
}