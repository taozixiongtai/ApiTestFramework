using ApiTestFramework.Domain.Enums;

namespace ApiTestFramework.Domain.Entities;

public class RequestItem
{
    public RequestVerbEnum RequestVerb { set; get; } = RequestVerbEnum.Get;

    public string Path { set; get; } = string.Empty;

    public string Body { set; get; } = string.Empty;

    public Dictionary<string, string> Header { set; get; } = [];
}
