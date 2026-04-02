namespace ApiTestFramework.Service.Interface;

/// <summary>
/// 种子数据服务接口，提供种子文件的管理和执行功能
/// </summary>
public interface ISeedDataService
{
    /// <summary>
    /// 获取 Seed 文件夹下所有 JSON 文件名
    /// </summary>
    /// <returns>文件名列表</returns>
    Task<List<string>> GetSeedFilesAsync();

    /// <summary>
    /// 保存 JSON 内容到 Seed 文件夹
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="content">JSON 内容</param>
    Task SaveSeedFileAsync(string fileName, string content);

    /// <summary>
    /// 执行种子数据插入到数据库
    /// </summary>
    Task ExecuteSeedDataAsync();

    /// <summary>
    /// 删除指定的种子文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    Task DeleteSeedFileAsync(string fileName);

    /// <summary>
    /// 获取指定文件的内容
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>文件内容</returns>
    Task<string> GetFileContentAsync(string fileName);
}
