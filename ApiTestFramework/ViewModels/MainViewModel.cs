using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Models;
using ApiTestFramework.Service.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace ApiTestFramework.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private RequestTreeViewModel _treeViewModel;

    [ObservableProperty]
    private RequestDetailViewModel _detailViewModel;

    [ObservableProperty]
    private SettingsViewModel _settingsViewModel;

    [ObservableProperty]
    private bool _isSettingsOpen;

    public MainViewModel(
        IRepository<GlobalSettings> settingsRepository,
        IRepository<List<RequestTreeItem>> treeRepository)
    {
        TreeViewModel = new RequestTreeViewModel(treeRepository);
        DetailViewModel = new RequestDetailViewModel();
        SettingsViewModel = new SettingsViewModel(settingsRepository);

        TreeViewModel.NodeSelected += OnNodeSelected;
    }

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

    [RelayCommand]
    private void OpenSettings()
    {
        IsSettingsOpen = true;
    }

    [RelayCommand]
    private void CloseSettings()
    {
        IsSettingsOpen = false;
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        await SettingsViewModel.SaveAsync();
        IsSettingsOpen = false;
        MessageBox.Show("设置已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private async Task SaveRequest()
    {
        DetailViewModel.SyncToNode();
        await TreeViewModel.SaveToDataAsync();
        MessageBox.Show("请求已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
