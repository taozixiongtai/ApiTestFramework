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
    /// 主界面当前选中的 Tab 页索引（0=请求，1=录制）
    /// </summary>
    [ObservableProperty]
    private int _selectedTabIndex;

    public MainViewModel(
        IHttpClientService httpClientService,
        IRepository<List<RequestTreeItem>> treeRepository,
        WebRecorderViewModel webRecorderViewModel,
        ReplayViewModel replayViewModel)
    {
        TreeViewModel = new RequestTreeViewModel(treeRepository);
        DetailViewModel = new RequestDetailViewModel(httpClientService);
        WebRecorderViewModel = webRecorderViewModel;
        ReplayViewModel = replayViewModel;

        WeakReferenceMessenger.Default.Register<NodeSelectedMessage>(this, OnNodeSelected);
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
        else if (node is RequestFolder folder)
        {
            // 文件夹节点展示复现视图：按文件夹内顺序执行全部请求
            SelectedTabIndex = 0;
            ReplayViewModel.LoadFolder(folder);
            CurrentDetailView = new ReplayControl { DataContext = ReplayViewModel };
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
