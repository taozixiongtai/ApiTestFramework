using CommunityToolkit.Mvvm.ComponentModel;

namespace ApiTestFramework.Models;

/// <summary>
/// 种子数据文件项，用于在列表中展示文件信息
/// </summary>
public partial class SeedDataFileItem : ObservableObject
{
    /// <summary>
    /// 文件路径
    /// </summary>
    [ObservableProperty]
    private string _filePath = string.Empty;

    /// <summary>
    /// 文件名（包含扩展名）
    /// </summary>
    [ObservableProperty]
    private string _fileName = string.Empty;

    /// <summary>
    /// 文件是否存在
    /// </summary>
    [ObservableProperty]
    private bool _fileExists = true;
}
