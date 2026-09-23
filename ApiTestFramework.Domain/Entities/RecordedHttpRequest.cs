namespace ApiTestFramework.Domain.Entities;

/// <summary>
/// 录制捕获的 HTTP 请求
/// </summary>
public class RecordedHttpRequest
{
    /// <summary>唯一标识</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>捕获顺序（从 1 开始）</summary>
    public int Order { get; set; }
    /// <summary>HTTP 方法（GET/POST 等，字符串以支持任意动词）</summary>
    public string Method { get; set; } = "GET";
    /// <summary>完整请求 URL</summary>
    public string Url { get; set; } = string.Empty;
    /// <summary>请求头</summary>
    public Dictionary<string, string> Headers { get; set; } = [];
    /// <summary>请求体（文本）</summary>
    public string Body { get; set; } = string.Empty;
    /// <summary>响应状态码（0 表示未获取到）</summary>
    public int StatusCode { get; set; }
    /// <summary>捕获时间</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
