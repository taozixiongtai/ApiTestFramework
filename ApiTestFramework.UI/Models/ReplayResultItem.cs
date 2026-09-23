using ApiTestFramework.Domain.Entities;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ApiTestFramework.UI.Models;

/// <summary>
/// 单个请求的复现结果条目，用于复现界面的逐条展示
/// </summary>
public partial class ReplayResultItem : ObservableObject
{
    /// <summary>
    /// 执行顺序
    /// </summary>
    [ObservableProperty]
    private int _order;

    /// <summary>
    /// HTTP 方法
    /// </summary>
    [ObservableProperty]
    private string _method = string.Empty;

    /// <summary>
    /// 请求 URL
    /// </summary>
    [ObservableProperty]
    private string _url = string.Empty;

    /// <summary>
    /// 响应状态码（0 表示网络异常未获得响应）
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusCodeText))]
    private int _statusCode;

    /// <summary>
    /// 耗时（毫秒）
    /// </summary>
    [ObservableProperty]
    private double _elapsedMs;

    /// <summary>
    /// 是否成功
    /// </summary>
    [ObservableProperty]
    private bool _isSuccess;

    /// <summary>
    /// 错误信息
    /// </summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// 状态码显示文本（0 时显示 "-"）
    /// </summary>
    public string StatusCodeText => StatusCode == 0 ? "-" : StatusCode.ToString();

    /// <summary>
    /// 从领域实体转换
    /// </summary>
    /// <param name="result">复现结果实体</param>
    /// <returns>视图模型条目</returns>
    public static ReplayResultItem FromDomain(ReplayResult result) => new()
    {
        Order = result.Order,
        Method = result.Method,
        Url = result.Url,
        StatusCode = result.StatusCode,
        ElapsedMs = result.ElapsedMs,
        IsSuccess = result.Success,
        ErrorMessage = result.ErrorMessage
    };
}
