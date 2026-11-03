using ApiTestFramework.Domain.Enums;

namespace ApiTestFramework.Domain.Models;

/// <summary>
/// 请求业务模型：单个 HTTP 请求的定义（动词/地址/请求头/请求体）
/// </summary>
public class RequestItem
{
    public RequestVerbEnum RequestVerb { set; get; } = RequestVerbEnum.Get;

    public string Path { set; get; } = string.Empty;

    public string Body { set; get; } = string.Empty;

    public Dictionary<string, string> Header { set; get; } = [];
}
