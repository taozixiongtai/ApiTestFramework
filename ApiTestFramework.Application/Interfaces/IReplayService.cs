using ApiTestFramework.Domain.Models;

namespace ApiTestFramework.Application.Interfaces;

/// <summary>
/// 请求文件夹复现服务，按现有顺序依次发送 HTTP 请求
/// </summary>
public interface IReplayService
{
    /// <summary>
    /// 执行请求列表的复现
    /// </summary>
    /// <param name="requests">要复现的请求列表（按列表顺序执行）</param>
    /// <param name="onResultCompleted">单个请求完成后的回调（在调用线程同步执行，用于 UI 逐条刷新）</param>
    /// <returns>表示异步操作的任务</returns>
    Task ExecuteAsync(IReadOnlyList<RequestItem> requests, Action<ReplayResult>? onResultCompleted = null);
}
