namespace ApiTestFramework.Domain.Entities;

/// <summary>
/// 单个请求的复现结果
/// </summary>
public class ReplayResult
{
    /// <summary>执行顺序</summary>
    public int Order { get; set; }
    /// <summary>HTTP 方法</summary>
    public string Method { get; set; } = string.Empty;
    /// <summary>请求 URL</summary>
    public string Url { get; set; } = string.Empty;
    /// <summary>响应状态码（0 表示网络异常未获得响应）</summary>
    public int StatusCode { get; set; }
    /// <summary>耗时（毫秒）</summary>
    public double ElapsedMs { get; set; }
    /// <summary>是否成功（状态码为 200）</summary>
    public bool Success { get; set; }
    /// <summary>错误信息</summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
