using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.Domain.Models;
using System.Diagnostics;
using System.Text.Json;

namespace ApiTestFramework.Application.Services;

/// <summary>
/// 请求文件夹复现服务：与请求详情页【发送】按钮共用同一发送链路（IHttpClientService），按列表顺序串行多次发送
/// </summary>
public class ReplayService : IReplayService
{
    private readonly IHttpClientService _httpClientService;

    public ReplayService(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    /// <summary>
    /// 执行请求列表的复现
    /// </summary>
    /// <param name="requests">要复现的请求列表（按列表顺序执行）</param>
    /// <param name="onResultCompleted">单个请求完成后的回调（在调用线程同步执行，用于 UI 逐条刷新）</param>
    /// <returns>表示异步操作的任务</returns>
    public async Task ExecuteAsync(IReadOnlyList<RequestItem> requests, Action<ReplayResult>? onResultCompleted = null)
    {
        for (var i = 0; i < requests.Count; i++)
        {
            var result = await ExecuteSingleAsync(requests[i], i + 1);
            onResultCompleted?.Invoke(result);
        }
    }

    /// <summary>
    /// 复现单个请求：按方法分发到 IHttpClientService，与【发送】按钮行为完全一致
    /// </summary>
    /// <param name="item">要复现的请求</param>
    /// <param name="order">执行顺序号（从 1 开始）</param>
    /// <returns>复现结果</returns>
    private async Task<ReplayResult> ExecuteSingleAsync(RequestItem item, int order)
    {
        var method = item.RequestVerb.ToString().ToUpperInvariant();

        var result = new ReplayResult
        {
            Order = order,
            Method = method,
            Url = item.Path
        };

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var body = GetRequestBody(item.Body);
            await (method switch
            {
                "GET" => _httpClientService.GetStringAsync(item.Path),
                "POST" => _httpClientService.PostStringAsync(item.Path, body!),
                "PUT" => _httpClientService.PutStringAsync(item.Path, body!),
                "DELETE" => _httpClientService.DeleteStringAsync(item.Path),
                "PATCH" => _httpClientService.PatchStringAsync(item.Path, body!),
                _ => throw new NotSupportedException($"不支持的请求方法: {method}")
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
