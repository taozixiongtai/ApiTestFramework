using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Service.Interface;
using System.Text.Json;

namespace ApiTestFramework.Service.Services;

public class DataService : IDataService
{
    private readonly string _dataFilePath;
    private AppData _appData;
    private readonly JsonSerializerOptions _jsonOptions;

    public DataService(string? dataFilePath = null)
    {
        _dataFilePath = dataFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appdata.json");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _appData = new AppData();
    }

    public AppData AppData => _appData;

    public async Task InitializeAsync()
    {
        if (File.Exists(_dataFilePath))
        {
            await LoadAsync();
        }
        else
        {
            _appData = new AppData();
            await SaveAsync();
        }
    }

    public async Task LoadAsync()
    {
        if (!File.Exists(_dataFilePath))
        {
            _appData = new AppData();
            return;
        }

        var json = await File.ReadAllTextAsync(_dataFilePath);
        var data = JsonSerializer.Deserialize<AppData>(json, _jsonOptions);
        _appData = data ?? new AppData();
    }

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(_appData, _jsonOptions);
        await File.WriteAllTextAsync(_dataFilePath, json);
    }

    public List<RequestTreeItem> GetRequestTree() => _appData.RequestTree;

    public async Task SaveRequestTreeAsync(List<RequestTreeItem> tree)
    {
        _appData.RequestTree = tree;
        await SaveAsync();
    }

    public GlobalSettings GetSettings() => _appData.Settings;

    public async Task SaveSettingsAsync(GlobalSettings settings)
    {
        _appData.Settings = settings;
        await SaveAsync();
    }

    public async Task UpdateTokenAsync(string token)
    {
        _appData.Settings.Token = token;
        await SaveAsync();
    }

    public async Task UpdateVariablesAsync(Dictionary<string, string> variables)
    {
        _appData.Settings.Variables = variables;
        await SaveAsync();
    }

    public async Task UpdateGlobalHeadersAsync(Dictionary<string, string> headers)
    {
        _appData.Settings.GlobalHeaders = headers;
        await SaveAsync();
    }

    public async Task UpdateBaseUrlAsync(string baseUrl)
    {
        _appData.Settings.BaseUrl = baseUrl;
        await SaveAsync();
    }
}
