using ApiTestFramework.UI.Controls;
using ApiTestFramework.Domain.Entities;
using ApiTestFramework.UI.Models;
using ApiTestFramework.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    private UserControl _currentDetailView = new EmptyControl();

    public MainViewModel(
        IHttpClientService httpClientService,
        IRepository<List<RequestTreeItem>> treeRepository,
        SeedDataDetailViewModel seedDataDetailViewModel)
    {
        TreeViewModel = new RequestTreeViewModel(treeRepository);
        DetailViewModel = new RequestDetailViewModel(httpClientService);
        SeedDataDetailViewModel = seedDataDetailViewModel;

        TreeViewModel.NodeSelected += OnNodeSelected;
    }

    private void OnNodeSelected(RequestNode node)
    {
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
        await TreeViewModel.SaveToDataAsync();
        MessageBox.Show("数据已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
