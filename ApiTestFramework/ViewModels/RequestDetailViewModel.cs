using ApiTestFramework.Infrastructure.Enum;
using ApiTestFramework.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ApiTestFramework.ViewModels;

public partial class RequestDetailViewModel : ObservableObject
{
    private RequestItemNode? _currentRequest;

    [ObservableProperty]
    private bool _hasRequest;

    [ObservableProperty]
    private string _requestName = string.Empty;

    [ObservableProperty]
    private RequestVerbEnum _requestVerb = RequestVerbEnum.Get;

    [ObservableProperty]
    private string _path = string.Empty;

    [ObservableProperty]
    private string _body = string.Empty;

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _headers = new();

    [ObservableProperty]
    private string _response = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public Array AvailableVerbs => Enum.GetValues(typeof(RequestVerbEnum));

    public void LoadRequest(RequestItemNode request)
    {
        _currentRequest = request;
        HasRequest = true;
        RequestName = request.Name;
        RequestVerb = request.RequestVerb;
        Path = request.Path;
        Body = request.Body;
        Headers.Clear();
        foreach (var header in request.Headers)
        {
            Headers.Add(header);
        }
        Response = string.Empty;
    }

    public void Clear()
    {
        _currentRequest = null;
        HasRequest = false;
        RequestName = string.Empty;
        RequestVerb = RequestVerbEnum.Get;
        Path = string.Empty;
        Body = string.Empty;
        Headers.Clear();
        Response = string.Empty;
    }

    public void SyncToNode()
    {
        if (_currentRequest == null)
        {
            return;
        }

        _currentRequest.Name = RequestName;
        _currentRequest.RequestVerb = RequestVerb;
        _currentRequest.Path = Path;
        _currentRequest.Body = Body;
        _currentRequest.Headers.Clear();
        foreach (var header in Headers)
        {
            _currentRequest.Headers.Add(header);
        }
    }

    public RequestItemNode? GetCurrentRequest()
    {
        return _currentRequest;
    }

    [RelayCommand]
    private async Task SendRequest()
    {
        if (string.IsNullOrWhiteSpace(Path))
        {
            Response = "请输入请求路径";
            return;
        }

        IsLoading = true;
        Response = "请求发送中...";

        try
        {
            await Task.Delay(1000);
            Response = $"请求已发送\n方法: {RequestVerb}\n路径: {Path}\n状态: 200 OK";
        }
        catch (Exception ex)
        {
            Response = $"请求失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void AddHeader()
    {
        Headers.Add(new KeyValuePair<string, string>("", ""));
    }

    public void RemoveHeader(KeyValuePair<string, string> header)
    {
        Headers.Remove(header);
    }
}
