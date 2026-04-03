using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;

namespace ApiTestFramework.UI.ViewModels;

public partial class FilePreviewViewModel : ObservableObject
{
    private readonly string _filePath;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _fileContent = string.Empty;

    public event Action? Saved;
    public event Action? Cancelled;

    public FilePreviewViewModel(string filePath)
    {
        _filePath = filePath;
        FileName = Path.GetFileName(filePath);
        FileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
    }

    [RelayCommand]
    private async Task Save()
    {
        try
        {
            await File.WriteAllTextAsync(_filePath, FileContent);
            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"保存失败: {ex.Message}", "错误", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        Cancelled?.Invoke();
    }
}
