namespace ApiTestFramework.Infrastructure.Json;

public class JsonTransformPipeline(IEnumerable<IJsonTransform> transforms)
{

    public string Execute(string json)
    {
        return transforms.Aggregate(json, (current, transform) => transform.Transform(current));
    }
}
