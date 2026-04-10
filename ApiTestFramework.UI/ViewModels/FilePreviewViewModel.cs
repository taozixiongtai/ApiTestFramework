using ApiTestFramework.UI.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.IO;
using System.Windows;

namespace ApiTestFramework.UI.ViewModels;

public partial class FilePreviewViewModel : ObservableObject
{
    private readonly string _filePath;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _fileContent = string.Empty;

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
            WeakReferenceMessenger.Default.Send(new FileSavedMessage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存失败: {ex.Message}", "错误", 
             MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        WeakReferenceMessenger.Default.Send(new FileCancelledMessage());
    }
}