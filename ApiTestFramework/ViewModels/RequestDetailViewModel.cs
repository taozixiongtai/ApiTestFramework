using ApiTestFramework.Infrastructure.Enum;
using ApiTestFramework.Models;
using ApiTestFramework.Service.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace ApiTestFramework.ViewModels;

/// <summary>
/// 请求详情视图模型，管理右侧请求详情的显示和编辑
/// </summary>
/// <remarks>
/// <para>该类负责管理单个 HTTP 请求的完整配置，包括：</para>
/// <list type="bullet">
///   <item><description>请求方法、路径、请求体和请求头</description></item>
///   <item><description>发送 HTTP 请求并显示响应结果</description></item>
///   <item><description>与 RequestItemNode 双向同步数据</description></item>
/// </list>
/// </remarks>
public partial class RequestDetailViewModel : ObservableObject
{
    private readonly IHttpClientService _httpClientService;
    private RequestItemNode? _currentRequest;

    /// <summary>
    /// 指示当前是否有请求被选中
    /// </summary>
    [ObservableProperty]
    private bool _hasRequest;

    /// <summary>
    /// 请求名称
    /// </summary>
    [ObservableProperty]
    private string _requestName = string.Empty;

    /// <summary>
    /// HTTP 请求方法
    /// </summary>
    [ObservableProperty]
    private RequestVerbEnum _requestVerb = RequestVerbEnum.Get;

    /// <summary>
    /// 请求路径
    /// </summary>
    [ObservableProperty]
    private string _path = string.Empty;

    /// <summary>
    /// 请求体内容
    /// </summary>
    [ObservableProperty]
    private string _body = string.Empty;

    /// <summary>
    /// 请求头集合
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _headers = new();

    /// <summary>
    /// 响应内容
    /// </summary>
    [ObservableProperty]
    private string _response = string.Empty;

    /// <summary>
    /// 指示是否正在发送请求
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _statusCode;

    [ObservableProperty]
    private double _responseTime;

    [ObservableProperty]
    private bool _hasResponse;

    public Array AvailableVerbs => Enum.GetValues(typeof(RequestVerbEnum));

    /// <summary>
    /// 初始化 RequestDetailViewModel 的新实例
    /// </summary>
    /// <param name="httpClientService">HTTP 客户端服务，用于发送 HTTP 请求</param>
    public RequestDetailViewModel(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    /// <summary>
    /// 加载请求节点的数据到视图模型
    /// </summary>
    /// <param name="request">要加载的请求节点</param>
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
        Response = request.Response;
        StatusCode = request.StatusCode;
        ResponseTime = request.ResponseTime;
        HasResponse = request.HasResponse;
    }

    /// <summary>
    /// 清空当前请求的所有数据
    /// </summary>
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

    /// <summary>
    /// 将视图模型的数据同步回请求节点
    /// </summary>
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

    /// <summary>
    /// 获取当前请求节点
    /// </summary>
    /// <returns>当前请求节点，如果没有选中则返回 null</returns>
    public RequestItemNode? GetCurrentRequest()
    {
        return _currentRequest;
    }

    /// <summary>
    /// 发送 HTTP 请求
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// <para>发送流程：</para>
    /// <list type="number">
    ///   <item><description>验证请求路径是否有效</description></item>
    ///   <item><description>根据请求方法调用对应的 HTTP 方法</description></item>
    ///   <item><description>显示响应结果或异常信息</description></item>
    /// </list>
    /// </remarks>
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

    /// <summary>
    /// 获取请求体对象
    /// </summary>
    /// <returns>请求体对象，如果请求体为空则返回 null</returns>
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

    /// <summary>
    /// 格式化 JSON 字符串
    /// </summary>
    /// <param name="json">JSON 字符串</param>
    /// <returns>格式化后的 JSON 字符串</returns>
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

    /// <summary>
    /// 添加一个新的请求头
    /// </summary>
    public void AddHeader()
    {
        Headers.Add(new KeyValuePair<string, string>("", ""));
    }

    /// <summary>
    /// 移除指定的请求头
    /// </summary>
    /// <param name="header">要移除的请求头</param>
    public void RemoveHeader(KeyValuePair<string, string> header)
    {
        Headers.Remove(header);
    }
}
