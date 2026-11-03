using ApiTestFramework.UI.Controls;
using ApiTestFramework.Infrastructure.Database;
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
    private ReplayViewModel _replayViewModel;

    [ObservableProperty]
    private UserControl _currentDetailView = new EmptyControl();

    public MainViewModel(
        IHttpClientService httpClientService,
        ITreeRepository treeRepository,
        ReplayViewModel replayViewModel)
    {
        TreeViewModel = new RequestTreeViewModel(treeRepository);
        DetailViewModel = new RequestDetailViewModel(httpClientService);
        ReplayViewModel = replayViewModel;

        WeakReferenceMessenger.Default.Register<NodeSelectedMessage>(this, OnNodeSelected);
    }

    private void OnNodeSelected(object recipient, NodeSelectedMessage message)
    {
        var node = message.Node;

        DetailViewModel.SyncToNode();

        if (node is RequestItemNode request)
        {
            DetailViewModel.LoadRequest(request);
            var control = new RequestDetailControl { DataContext = DetailViewModel };
            CurrentDetailView = control;
        }
        else if (node is RequestFolder folder)
        {
            // 文件夹节点展示复现视图：按文件夹内顺序执行全部请求
            ReplayViewModel.LoadFolder(folder);
            CurrentDetailView = new ReplayControl { DataContext = ReplayViewModel };
        }
        else
        {
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
