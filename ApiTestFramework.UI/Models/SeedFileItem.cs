using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace ApiTestFramework.UI.Models;

public partial class SeedFileItem : ObservableObject
{
    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private DateTime _uploadTime;

    [ObservableProperty]
    private long _fileSize;

    public string FileSizeDisplay
    {
        get
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            return FileSize switch
            {
                >= MB => $"{FileSize / (double)MB:F2} MB",
                >= KB => $"{FileSize / (double)KB:F2} KB",
                _ => $"{FileSize} B"
            };
        }
    }

    public static SeedFileItem FromFile(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        return new SeedFileItem
        {
            FileName = fileInfo.Name,
            FilePath = filePath,
            UploadTime = fileInfo.CreationTime,
            FileSize = fileInfo.Length
        };
    }
}
