using ApiTestFramework.Infrastructure.APP;
using ApiTestFramework.Service.Interface;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Text.Json;

namespace ApiTestFramework.Service.Services;

public class HttpClientService : IHttpClientService
{
    private readonly RestClient _client;
    private string? _jwtToken;
    private readonly AppOption _appOption;
    private readonly Dictionary<string, string> _requestHeader;

    public HttpClientService(IOptions<AppOption> options)
    {
        var clientOptions = new RestClientOptions
        {
            ThrowOnAnyError = false,
            UserAgent = "MyWPFApp/1.0"
        };


        _appOption = options.Value;
        _requestHeader = _appOption.RequestHeader ?? [];
        _client = new RestClient(clientOptions);

    }

    public async Task<T> GetAsync<T>(string url)
    {
        var request = await CreateRequest(url, Method.Get);
        var result = await _client.ExecuteAsync<T>(request);
        return HandlerResult(result);
    }

    public async Task<T> PostAsync<T>(string url, object body)
    {
        var request = await CreateRequest(url, Method.Post);
        request.AddJsonBody(body);
        var result = await _client.ExecuteAsync<T>(request);
        return HandlerResult(result);
    }

    public async Task<T> PutAsync<T>(string url, object body)
    {
        var request = await CreateRequest(url, Method.Put);
        request.AddJsonBody(body);
        var result = await _client.ExecuteAsync<T>(request);
        return HandlerResult(result);
    }

    public async Task<T> DeleteAsync<T>(string url)
    {
        var request = await CreateRequest(url, Method.Delete);
        var result = await _client.ExecuteAsync<T>(request);
        return HandlerResult(result);
    }

    private T HandlerResult<T>(RestResponse<T> response)
    {
        if (!response.IsSuccessful)
        {
            throw new Exception($"请求调用失败 {response.StatusCode}: {response.ErrorMessage}");
        }
        return response.Data!;
    }

    private async Task<RestRequest> CreateRequest(string url, Method method)
    {
        var request = new RestRequest(url, method);

        if (_jwtToken is null)
        {
            await SetToken();
        }

        // 设置header
        if (_requestHeader.Count > 0)
        {
            foreach (var header in _requestHeader)
            {
                request.AddHeader(header.Key, header.Value);
            }
        }

        return request;
    }

    private async Task SetToken()
    {
        // 设置一个值，避免重复请求
        _jwtToken = string.Empty;
        var tokenString = await PostAsync<string>(_appOption.LoginUrl, new
        {
            username = "",
            password = ""
        });


        JsonDocument doc = JsonDocument.Parse(tokenString);
        if (doc.RootElement.TryGetProperty("data", out JsonElement dataElement) &&
           dataElement.TryGetProperty("accessToken", out JsonElement tokenElement))
        {
            _jwtToken = tokenElement.GetString();
        }
        else
        {
            throw new Exception("登录接口返回结果异常，无法获取到 Token");
        }

        // 将 Token 添加到默认请求头中
        _requestHeader.Add("Authorization", $"Bearer {_jwtToken}");
    }
}
