using ApiTestFramework.Infrastructure.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ApiTestFramework.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty]
    private string _token = string.Empty;

    [ObservableProperty]
    private string _baseUrl = string.Empty;

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _globalVariables = new();

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _globalHeaders = new();

    public SettingsViewModel(DataService dataService)
    {
        _dataService = dataService;
        LoadFromData();
    }

    private void LoadFromData()
    {
        var settings = _dataService.GetSettings();

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
    public async Task SaveAsync()
    {
        var settings = _dataService.GetSettings();
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

        await _dataService.SaveSettingsAsync(settings);
    }

    public void AddVariable()
    {
        GlobalVariables.Add(new KeyValuePair<string, string>("", ""));
    }

    public void RemoveVariable(KeyValuePair<string, string> variable)
    {
        GlobalVariables.Remove(variable);
    }

    public void AddHeader()
    {
        GlobalHeaders.Add(new KeyValuePair<string, string>("", ""));
    }

    public void RemoveHeader(KeyValuePair<string, string> header)
    {
        GlobalHeaders.Remove(header);
    }
}
