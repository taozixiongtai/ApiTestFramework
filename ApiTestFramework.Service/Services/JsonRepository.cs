using System.Text.Json;

namespace ApiTestFramework.Service.Services;

public class JsonRepository<T> : Service.Interface.IRepository<T> where T : class, new()
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private T? _cache;

    public JsonRepository()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var typeName = typeof(T).Name;
        _filePath = Path.Combine(basePath, $"{typeName}.json");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T> GetAsync()
    {
        if (_cache != null)
        {
            return _cache;
        }

        if (!File.Exists(_filePath))
        {
            _cache = new T();
            return _cache;
        }

        var json = await File.ReadAllTextAsync(_filePath);
        _cache = JsonSerializer.Deserialize<T>(json, _jsonOptions) ?? new T();
        return _cache;
    }

    public async Task SaveAsync(T entity)
    {
        _cache = entity;
        var json = JsonSerializer.Serialize(entity, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task ResetAsync()
    {
        _cache = new T();
        var json = JsonSerializer.Serialize(_cache, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, json);
    }
}
