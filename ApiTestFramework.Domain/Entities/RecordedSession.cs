namespace ApiTestFramework.Domain.Entities;

/// <summary>
/// 录制会话，保存一次手动操作捕获的全部请求
/// </summary>
public class RecordedSession
{
    /// <summary>唯一标识</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>会话名称</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    /// <summary>捕获的请求列表（按 Order 排序执行）</summary>
    public List<RecordedHttpRequest> Requests { get; set; } = [];
}
