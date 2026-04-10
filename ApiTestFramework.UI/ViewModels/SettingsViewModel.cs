using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ApiTestFramework.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IRepository<GlobalSettings> _settingsRepository;

    [ObservableProperty]
    private string _token = string.Empty;

    [ObservableProperty]
    private string _baseUrl = string.Empty;

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _globalVariables = [];

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _globalHeaders = [];

    public event Action? Saved;
    public event Action? Cancelled;

    public SettingsViewModel(IRepository<GlobalSettings> settingsRepository)
    {
        _settingsRepository = settingsRepository;
        LoadFromData();
    }

    private async void LoadFromData()
    {
        var settings = await _settingsRepository.GetAsync();

        Token = settings.Token;
        BaseUrl = settings.BaseUrl;

        GlobalVariables.Clear();
        foreach (var kv in settings.Variables)
        {
            GlobalVariables.Add(kv);
        }

        GlobalHeaders.Clear();
        foreach (var kv in settings.GlobalHeaders)
        {
            GlobalHeaders.Add(kv);
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        var settings = await _settingsRepository.GetAsync();
        settings.Token = Token;
        settings.BaseUrl = BaseUrl;

        settings.Variables.Clear();
        foreach (var kv in GlobalVariables)
        {
            if (!string.IsNullOrWhiteSpace(kv.Key))
            {
                settings.Variables[kv.Key] = kv.Value;
            }
        }

        settings.GlobalHeaders.Clear();
        foreach (var kv in GlobalHeaders)
        {
            if (!string.IsNullOrWhiteSpace(kv.Key))
            {
                settings.GlobalHeaders[kv.Key] = kv.Value;
            }
        }

        await _settingsRepository.SaveAsync(settings);
        Saved?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        Cancelled?.Invoke();
    }

    [RelayCommand]
    private void AddVariable()
    {
        GlobalVariables.Add(new KeyValuePair<string, string>("", ""));
    }

    [RelayCommand]
    private void RemoveVariable(KeyValuePair<string, string> variable)
    {
        GlobalVariables.Remove(variable);
    }

    [RelayCommand]
    private void AddHeader()
    {
        GlobalHeaders.Add(new KeyValuePair<string, string>("", ""));
    }

    [RelayCommand]
    private void RemoveHeader(KeyValuePair<string, string> header)
    {
        GlobalHeaders.Remove(header);
    }
}
