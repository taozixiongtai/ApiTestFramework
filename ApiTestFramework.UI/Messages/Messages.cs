using ApiTestFramework.Domain.Entities;
using ApiTestFramework.UI.Models;

namespace ApiTestFramework.UI.Messages;

public sealed record NodeSelectedMessage(RequestNode Node);

public sealed record CreateRequestMessage;

public sealed record RequestCreatedMessage(RequestItemNode Request);

public sealed record SaveDataMessage;

/// <summary>
/// 录制保存消息，携带录制会话，由请求树接收并生成对应的请求文件夹
/// </summary>
public sealed record SaveRecordingToTreeMessage(RecordedSession Session);
