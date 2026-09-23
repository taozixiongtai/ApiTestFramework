using ApiTestFramework.Domain.Entities;

namespace ApiTestFramework.Application.Interfaces;

/// <summary>
/// 录制会话复现服务，按录制顺序依次发送 HTTP 请求
/// </summary>
public interface IReplayService
{
    /// <summary>
    /// 执行录制会话的复现
    /// </summary>
    /// <param name="session">要复现的录制会话</param>
    /// <param name="onResultCompleted">单个请求完成后的回调（在调用线程同步执行，用于 UI 逐条刷新）</param>
    /// <returns>表示异步操作的任务</returns>
    Task ExecuteAsync(RecordedSession session, Action<ReplayResult>? onResultCompleted = null);
}
