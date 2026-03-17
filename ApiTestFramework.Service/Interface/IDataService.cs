using ApiTestFramework.Infrastructure.Domain;

namespace ApiTestFramework.Service.Interface;

public interface IDataService
{
    AppData AppData { get; }

    Task InitializeAsync();
    Task LoadAsync();
    Task SaveAsync();
    List<RequestTreeItem> GetRequestTree();
    Task SaveRequestTreeAsync(List<RequestTreeItem> tree);
    GlobalSettings GetSettings();
    Task SaveSettingsAsync(GlobalSettings settings);
    Task UpdateTokenAsync(string token);
    Task UpdateVariablesAsync(Dictionary<string, string> variables);
    Task UpdateGlobalHeadersAsync(Dictionary<string, string> headers);
    Task UpdateBaseUrlAsync(string baseUrl);
}
