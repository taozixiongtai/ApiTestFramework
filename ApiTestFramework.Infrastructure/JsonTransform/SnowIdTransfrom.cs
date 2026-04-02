using ApiTestFramework.Infrastructure.Helper;

namespace ApiTestFramework.Infrastructure.JsonTransform;

public class SnowIdTransfrom : IJsonTransform
{
    public string Transform(string json)
    {
        return json.Replace($"#SnowId#", SnowflakeIdGenerator.NextId().ToString());
    }
}
