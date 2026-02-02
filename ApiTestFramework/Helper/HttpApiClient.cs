using RestSharp;
using System.Text.Json;

namespace ApiTestFramework.Helper;

public class HttpApiClient(string baseUrl)
{
    private readonly RestClient _client = new RestClient(baseUrl);
    private string? _jwtToken;
    private readonly string _longinUrl = "/api/system/security/login";

    /// <summary>
    /// 设置 JWT Token
    /// </summary>
    public async Task SetJwt()
    {
        if (_jwtToken != null)
        {
            return;
        }

        var tokenString = await SendAsync<string>(
            _longinUrl,
            Method.Post,
            new
            {
                username = "admin",
                password = "Aa123456"
            }) ?? throw new Exception("登录接口返回结果为空");

        JsonDocument doc = JsonDocument.Parse(tokenString);
        if (doc.RootElement.TryGetProperty("data", out JsonElement dataElement) &&
           dataElement.TryGetProperty("accessToken", out JsonElement tokenElement))
        {
            _jwtToken = tokenElement.GetString();
        }

    }


    /// <summary>
    /// 统一请求入口
    /// </summary>
    public async Task<T?> SendAsync<T>(
        string url,
        Method method,
        object? body = null,
        Dictionary<string, string>? query = null,
        Dictionary<string, string>? headers = null)
    {

        await SetJwt();

        var request = new RestRequest(url, method);

        if (!string.IsNullOrEmpty(_jwtToken))
        {
            request.AddHeader("Authorization", $"Bearer {_jwtToken}");
        }

        if (query != null)
        {
            foreach (var kv in query)
            {
                request.AddQueryParameter(kv.Key, kv.Value);
            }
        }

        if (headers != null)
        {
            foreach (var kv in headers)
            {
                request.AddHeader(kv.Key, kv.Value);
            }
        }

        if (body != null && method != Method.Get)
        {
            request.AddJsonBody(body);
        }

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new Exception($"HTTP Error: {response.StatusCode} {response.Content}");
        }

        if (string.IsNullOrWhiteSpace(response.Content))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(response.Content);
    }


}
