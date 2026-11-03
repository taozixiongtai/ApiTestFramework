using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.Domain.Entities;
using System.Diagnostics;
using System.Text.Json;

namespace ApiTestFramework.Application.Services;

/// <summary>
/// 录制会话复现服务：与请求详情页【发送】按钮共用同一发送链路（IHttpClientService），按录制顺序串行多次发送
/// </summary>
public class ReplayService : IReplayService
{
    private readonly IHttpClientService _httpClientService;

    public ReplayService(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

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
    /// 复现单个录制的 HTTP 请求：按方法分发到 IHttpClientService，与【发送】按钮行为完全一致
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

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var body = GetRequestBody(item.Body);
            await (item.Method.ToUpperInvariant() switch
            {
                "GET" => _httpClientService.GetStringAsync(item.Url),
                "POST" => _httpClientService.PostStringAsync(item.Url, body!),
                "PUT" => _httpClientService.PutStringAsync(item.Url, body!),
                "DELETE" => _httpClientService.DeleteStringAsync(item.Url),
                "PATCH" => _httpClientService.PatchStringAsync(item.Url, body!),
                _ => throw new NotSupportedException($"不支持的请求方法: {item.Method}")
            });
            result.StatusCode = 200;
            result.Success = true;
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
    /// 解析请求体：JSON 字符串反序列化为对象（与【发送】按钮的 GetRequestBody 逻辑一致），非 JSON 原样返回
    /// </summary>
    private static object? GetRequestBody(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<object>(body);
        }
        catch
        {
            return body;
        }
    }
}
