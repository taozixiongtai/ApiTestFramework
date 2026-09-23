using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.Domain.Entities;
using RestSharp;
using System.Diagnostics;

namespace ApiTestFramework.Application.Services;

/// <summary>
/// 录制会话复现服务，按录制顺序依次发送 HTTP 请求
/// </summary>
public class ReplayService : IReplayService
{
    private readonly RestClient _client = new(new RestClientOptions { ThrowOnAnyError = false });

    /// <summary>
    /// 执行录制会话的复现
    /// </summary>
    /// <param name="session">要复现的录制会话</param>
    /// <param name="onResultCompleted">单个请求完成后的回调（在调用线程同步执行，用于 UI 逐条刷新）</param>
    /// <returns>表示异步操作的任务</returns>
    public async Task ExecuteAsync(RecordedSession session, Action<ReplayResult>? onResultCompleted = null)
    {
        foreach (var item in session.Requests.OrderBy(r => r.Order))
        {
            var result = await ExecuteSingleAsync(item);
            onResultCompleted?.Invoke(result);
        }
    }

    /// <summary>
    /// 复现单个录制的 HTTP 请求
    /// </summary>
    /// <param name="item">录制的 HTTP 请求</param>
    /// <returns>复现结果</returns>
    private async Task<ReplayResult> ExecuteSingleAsync(RecordedHttpRequest item)
    {
        var result = new ReplayResult
        {
            Order = item.Order,
            Method = item.Method,
            Url = item.Url
        };

        var method = ParseMethod(item.Method);
        var request = new RestRequest(item.Url, method);

        foreach (var header in item.Headers)
        {
            if (string.Equals(header.Key, "Content-Length", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(header.Key, "Host", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            if (!string.IsNullOrEmpty(header.Value))
            {
                request.AddHeader(header.Key, header.Value);
            }
        }

        if (!string.IsNullOrEmpty(item.Body))
        {
            request.AddStringBody(item.Body, DataFormat.Json);
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await _client.ExecuteAsync(request);
            result.StatusCode = (int)response.StatusCode;
            result.Success = result.StatusCode == 200;
        }
        catch (Exception ex)
        {
            result.StatusCode = 0;
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ElapsedMs = stopwatch.Elapsed.TotalMilliseconds;
        }

        return result;
    }

    /// <summary>
    /// 将录制的 HTTP 方法字符串解析为 RestSharp 方法
    /// </summary>
    /// <param name="method">HTTP 方法字符串</param>
    /// <returns>RestSharp 方法，无法识别时默认为 GET</returns>
    private static Method ParseMethod(string method)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => Method.Get,
            "POST" => Method.Post,
            "PUT" => Method.Put,
            "DELETE" => Method.Delete,
            "PATCH" => Method.Patch,
            "HEAD" => Method.Head,
            "OPTIONS" => Method.Options,
            _ => Method.Get
        };
    }
}
