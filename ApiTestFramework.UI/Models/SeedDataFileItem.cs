using CommunityToolkit.Mvvm.ComponentModel;

namespace ApiTestFramework.UI.Models;

public partial class SeedDataFileItem : ObservableObject
{
    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private bool _fileExists = true;
}
