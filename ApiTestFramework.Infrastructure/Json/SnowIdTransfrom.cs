using ApiTestFramework.Infrastructure.IdGenerator;

namespace ApiTestFramework.Infrastructure.Json;

public class SnowIdTransfrom : IJsonTransform
{
    public string Transform(string json)
    {
        return json.Replace($"#SnowId#", SnowflakeIdGenerator.NextId().ToString());
    }
}
