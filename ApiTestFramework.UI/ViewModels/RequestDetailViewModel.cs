using ApiTestFramework.Domain.Enums;
using ApiTestFramework.UI.Models;
using ApiTestFramework.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace ApiTestFramework.UI.ViewModels;

public partial class RequestDetailViewModel : ObservableObject
{
    private readonly IHttpClientService _httpClientService;
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

    [ObservableProperty]
    private int _statusCode;

    [ObservableProperty]
    private double _responseTime;

    [ObservableProperty]
    private bool _hasResponse;

    public Array AvailableVerbs => Enum.GetValues(typeof(RequestVerbEnum));

    public RequestDetailViewModel(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public void LoadRequest(object request)
    {
        if (request is RequestItemNode requestNode)
        {
            _currentRequest = requestNode;
            HasRequest = true;
            RequestName = requestNode.Name;
            RequestVerb = requestNode.RequestVerb;
            Path = requestNode.Path;
            Body = requestNode.Body;
            Headers.Clear();
            foreach (var header in requestNode.Headers)
            {
                Headers.Add(header);
            }
            Response = requestNode.Response;
            StatusCode = requestNode.StatusCode;
            ResponseTime = requestNode.ResponseTime;
            HasResponse = requestNode.HasResponse;
        }
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
        HasResponse = false;
        StatusCode = 0;
        ResponseTime = 0;
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
        var startTime = DateTime.Now;
        var sb = new StringBuilder();
        sb.AppendLine($"请求时间: {startTime:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"方法: {RequestVerb}");
        sb.AppendLine($"路径: {Path}");
        sb.AppendLine();

        try
        {
            string responseBody = RequestVerb switch
            {
                RequestVerbEnum.Get => await _httpClientService.GetStringAsync(Path),
                RequestVerbEnum.Post => await _httpClientService.PostStringAsync(Path, GetRequestBody()),
                RequestVerbEnum.Put => await _httpClientService.PutStringAsync(Path, GetRequestBody()),
                RequestVerbEnum.Delete => await _httpClientService.DeleteStringAsync(Path),
                RequestVerbEnum.Patch => await _httpClientService.PatchStringAsync(Path, GetRequestBody()),
                _ => throw new NotSupportedException($"不支持的请求方法: {RequestVerb}")
            };

            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            StatusCode = 200;
            ResponseTime = elapsed;
            HasResponse = true;
            sb.AppendLine($"状态: 200 OK");
            sb.AppendLine($"耗时: {elapsed:F2}ms");
            sb.AppendLine();
            sb.AppendLine("响应内容:");
            sb.AppendLine(FormatJson(responseBody));
        }
        catch (Exception ex)
        {
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            StatusCode = 0;
            ResponseTime = elapsed;
            HasResponse = true;
            sb.AppendLine($"状态: 请求失败");
            sb.AppendLine($"耗时: {elapsed:F2}ms");
            sb.AppendLine();
            sb.AppendLine("异常信息:");
            sb.AppendLine(ex.Message);

            if (ex.InnerException != null)
            {
                sb.AppendLine();
                sb.AppendLine("内部异常:");
                sb.AppendLine(ex.InnerException.Message);
            }
        }
        finally
        {
            IsLoading = false;
        }

        Response = sb.ToString();

        if (_currentRequest != null)
        {
            _currentRequest.Response = Response;
            _currentRequest.StatusCode = StatusCode;
            _currentRequest.ResponseTime = ResponseTime;
            _currentRequest.HasResponse = HasResponse;
        }
    }

    private object? GetRequestBody()
    {
        if (string.IsNullOrWhiteSpace(Body))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<object>(Body);
        }
        catch
        {
            return Body;
        }
    }

    private static string FormatJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return string.Empty;
        }

        try
        {
            var doc = JsonDocument.Parse(json);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(doc, options);
        }
        catch
        {
            return json;
        }
    }

    [RelayCommand]
    private void AddHeader()
    {
        Headers.Add(new KeyValuePair<string, string>("", ""));
    }

    [RelayCommand]
    private void RemoveHeader(KeyValuePair<string, string> header)
    {
        Headers.Remove(header);
    }
}
