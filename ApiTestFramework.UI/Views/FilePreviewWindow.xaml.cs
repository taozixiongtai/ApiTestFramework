using System.IO;
using System.Windows;

namespace ApiTestFramework.UI.Views;

public partial class FilePreviewWindow : Window
{
    private readonly string _filePath;

    public string FileName { get; }

    public string FileContent { get; set; }

    public FilePreviewWindow(string filePath)
    {
        InitializeComponent();
        _filePath = filePath;
        FileName = Path.GetFileName(filePath);
        FileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
        DataContext = this;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await File.WriteAllTextAsync(_filePath, FileContent);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
