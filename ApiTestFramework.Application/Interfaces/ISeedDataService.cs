namespace ApiTestFramework.Application.Interfaces;

public interface ISeedDataService
{
    Task<List<string>> GetSeedFilesAsync();
    Task SaveSeedFileAsync(string fileName, string content);
    Task ExecuteSeedDataAsync(string[] filePaths);
    Task ExecuteSeedDataAsync();
    Task DeleteSeedFileAsync(string fileName);
    Task<string> GetFileContentAsync(string fileName);
}
