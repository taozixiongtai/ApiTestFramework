using ApiTestFramework.Infrastructure.Helper;
using ApiTestFramework.Infrastructure.JsonTransform;
using ApiTestFramework.Service.Interface;

namespace ApiTestFramework.Service.Services;

/// <summary>
/// 种子数据服务实现，提供种子文件的管理和执行功能
/// </summary>
public class SeedDataService : ISeedDataService
{
    private readonly IDatabaseService _databaseService;
    private readonly JsonTransformPipeline _jsonTransformPipeline;
    private readonly string _seedPath;

    /// <summary>
    /// 初始化 SeedDataService 的新实例
    /// </summary>
    /// <param name="databaseService">数据库服务，用于插入数据</param>
    /// <param name="jsonTransformPipeline">JSON 转换管道，用于变量替换</param>
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

    /// <summary>
    /// 获取 Seed 文件夹下所有 JSON 文件名
    /// </summary>
    /// <returns>文件名列表</returns>
    public Task<List<string>> GetSeedFilesAsync()
    {
        var files = Directory.GetFiles(_seedPath, "*.json")
            .Select(Path.GetFileName)
            .Where(name => name != null)
            .Cast<string>()
            .ToList();

        return Task.FromResult(files);
    }

    /// <summary>
    /// 保存 JSON 内容到 Seed 文件夹
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="content">JSON 内容</param>
    public async Task SaveSeedFileAsync(string fileName, string content)
    {
        var filePath = Path.Combine(_seedPath, fileName);
        await File.WriteAllTextAsync(filePath, content);
    }

    /// <summary>
    /// 执行种子数据插入到数据库
    /// </summary>
    /// <param name="filePaths">要执行的文件路径列表</param>
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

    /// <summary>
    /// 执行种子数据插入到数据库（所有种子文件）
    /// </summary>
    public async Task ExecuteSeedDataAsync()
    {
        var files = Directory.GetFiles(_seedPath, "*.json");
        await ExecuteSeedDataAsync(files);
    }

    /// <summary>
    /// 删除指定的种子文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    public Task DeleteSeedFileAsync(string fileName)
    {
        var filePath = Path.Combine(_seedPath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取指定文件的内容
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>文件内容</returns>
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
