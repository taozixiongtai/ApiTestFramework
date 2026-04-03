using ApiTestFramework.UI.Models;

namespace ApiTestFramework.UI.Messages;

public sealed record NodeSelectedMessage(RequestNode Node);

public sealed record CreateRequestMessage;

public sealed record CreateSeedDataMessage;

public sealed record RequestCreatedMessage(RequestItemNode Request);

public sealed record SeedDataCreatedMessage(SeedDataNode SeedData);

public sealed record SaveDataMessage;

public sealed record FileSavedMessage;

public sealed record FileCancelledMessage;
