namespace ApiTestFramework.Infrastructure.JsonTransform;

/// <summary>
/// 责任链的管道
/// </summary>
/// <param name="transforms"></param>
public class JsonTransformPipeline(IEnumerable<IJsonTransform> transforms)
{

    public string Execute(string json)
    {
        return transforms.Aggregate(json, (current, transform) => transform.Transform(current));
    }
}
