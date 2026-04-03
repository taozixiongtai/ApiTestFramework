namespace ApiTestFramework.Domain.Entities;

public class GlobalSettings
{
    public string Token { get; set; } = string.Empty;

    public Dictionary<string, string> Variables { get; set; } = [];

    public Dictionary<string, string> GlobalHeaders { get; set; } = [];

    public string BaseUrl { get; set; } = string.Empty;
}
