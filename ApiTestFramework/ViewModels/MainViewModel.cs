using ApiTestFramework.Infrastructure.Service;
using ApiTestFramework.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace ApiTestFramework.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty]
    private RequestTreeViewModel _treeViewModel;

    [ObservableProperty]
    private RequestDetailViewModel _detailViewModel;

    [ObservableProperty]
    private SettingsViewModel _settingsViewModel;

    [ObservableProperty]
    private bool _isSettingsOpen;

    public MainViewModel(DataService dataService)
    {
        _dataService = dataService;

        TreeViewModel = new RequestTreeViewModel(_dataService);
        DetailViewModel = new RequestDetailViewModel();
        SettingsViewModel = new SettingsViewModel(_dataService);

        TreeViewModel.RequestSelected += OnRequestSelected;
    }

    private void OnRequestSelected(RequestItemNode request)
    {
        DetailViewModel.RequestName = request.Name;
        DetailViewModel.RequestVerb = request.RequestVerb;
        DetailViewModel.Path = request.Path;
        DetailViewModel.Body = request.Body;
        DetailViewModel.Headers.Clear();
        foreach (var header in request.Headers)
        {
            DetailViewModel.Headers.Add(header);
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
}
