namespace ApiTestFramework.Domain.Entities;

/// <summary>
/// 录制会话集合（独立类型以保证 JsonRepository 生成独立文件名，避免与 List&lt;RequestTreeItem&gt; 冲突）
/// </summary>
public class RecordedSessionCollection : List<RecordedSession>;
