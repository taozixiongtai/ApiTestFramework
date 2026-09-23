using ApiTestFramework.UI.Models;

namespace ApiTestFramework.UI.Messages;

public sealed record NodeSelectedMessage(RequestNode Node);

public sealed record CreateRequestMessage;

public sealed record RequestCreatedMessage(RequestItemNode Request);

public sealed record SaveDataMessage;

/// <summary>
/// 录制会话保存完成消息，通知左侧树刷新录制会话节点
/// </summary>
public sealed record RecordingSessionSavedMessage;
