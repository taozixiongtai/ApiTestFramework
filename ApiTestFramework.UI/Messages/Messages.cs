using ApiTestFramework.UI.Models;

namespace ApiTestFramework.UI.Messages;

public sealed record NodeSelectedMessage(RequestNode Node);

public sealed record CreateRequestMessage;

public sealed record RequestCreatedMessage(RequestItemNode Request);

public sealed record SaveDataMessage;
