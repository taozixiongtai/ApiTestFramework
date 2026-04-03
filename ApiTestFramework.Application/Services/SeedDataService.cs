using ApiTestFramework.Infrastructure.Json;
using ApiTestFramework.Application.Interfaces;

namespace ApiTestFramework.Application.Services;

public class SeedDataService : ISeedDataService
{
    private readonly IDatabaseService _databaseService;
    private readonly JsonTransformPipeline _jsonTransformPipeline;
    private readonly string _seedPath;

    public SeedDataService(IDatabaseService databaseService, JsonTransformPipeline jsonTransformPipeline)
    {
        _databaseService = databaseService;
        _jsonTransformPipeline = jsonTransformPipeline;
        _seedPath = Path.Combine(AppContext.BaseDirectory, "Seed");

        if (!Directory.Exists(_seedPath))
        {
            Directory.CreateDirectory(_seedPath);
        }
    }

    public Task<List<string>> GetSeedFilesAsync()
    {
        var files = Directory.GetFiles(_seedPath, "*.json")
            .Select(Path.GetFileName)
            .Where(name => name != null)
            .Cast<string>()
            .ToList();

        return Task.FromResult(files);
    }

    public async Task SaveSeedFileAsync(string fileName, string content)
    {
        var filePath = Path.Combine(_seedPath, fileName);
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task ExecuteSeedDataAsync(string[] filePaths)
    {
        foreach (var filePath in filePaths)
        {
            if (File.Exists(filePath))
            {
                var content = await File.ReadAllTextAsync(filePath);
                var transformedJson = _jsonTransformPipeline.Execute(content);
                var tableRecords = JsonHelper.ParseDirectory(transformedJson);

                foreach (var table in tableRecords)
                {
                    _databaseService.InsertData(table.Key, table.Value);
                }
            }
        }
    }

    public async Task ExecuteSeedDataAsync()
    {
        var files = Directory.GetFiles(_seedPath, "*.json");
        await ExecuteSeedDataAsync(files);
    }

    public Task DeleteSeedFileAsync(string fileName)
    {
        var filePath = Path.Combine(_seedPath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }

    public async Task<string> GetFileContentAsync(string fileName)
    {
        var filePath = Path.Combine(_seedPath, fileName);
        if (File.Exists(filePath))
        {
            return await File.ReadAllTextAsync(filePath);
        }
        return string.Empty;
    }
}
