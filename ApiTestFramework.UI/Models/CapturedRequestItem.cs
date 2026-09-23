using ApiTestFramework.Domain.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ApiTestFramework.UI.Models;

/// <summary>
/// 录制捕获的请求条目，用于 Web 录制界面的实时展示
/// </summary>
public partial class CapturedRequestItem : ObservableObject
{
    /// <summary>
    /// 捕获顺序（从 1 开始）
    /// </summary>
    [ObservableProperty]
    private int _order;

    /// <summary>
    /// HTTP 方法（GET/POST 等）
    /// </summary>
    [ObservableProperty]
    private string _method = string.Empty;

    /// <summary>
    /// 完整请求 URL
    /// </summary>
    [ObservableProperty]
    private string _url = string.Empty;

    /// <summary>
    /// 响应状态码（0 表示尚未收到响应）
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCodeText))]
    private int _statusCode;

    /// <summary>
    /// 请求体（文本）
    /// </summary>
    [ObservableProperty]
    private string _body = string.Empty;

    /// <summary>
    /// 请求头
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _headers = [];

    /// <summary>
    /// 捕获时间
    /// </summary>
    [ObservableProperty]
    private DateTime _timestamp = DateTime.Now;

    /// <summary>
    /// 状态码显示文本（0 时显示 "-" 表示等待响应）
    /// </summary>
    public string StatusCodeText => StatusCode == 0 ? "-" : StatusCode.ToString();

    /// <summary>
    /// 从录制请求实体转换（复现结果查看详情时使用）
    /// </summary>
    /// <param name="request">录制请求实体</param>
    /// <returns>视图模型条目</returns>
    public static CapturedRequestItem FromDomain(RecordedHttpRequest request) => new()
    {
        Order = request.Order,
        Method = request.Method,
        Url = request.Url,
        StatusCode = request.StatusCode,
        Body = request.Body,
        Headers = new ObservableCollection<KeyValuePair<string, string>>(request.Headers),
        Timestamp = request.Timestamp
    };
}
