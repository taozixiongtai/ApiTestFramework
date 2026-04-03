using System.IO;
using System.Windows;

namespace ApiTestFramework;

/// <summary>
/// 文件预览窗口，用于查看和编辑文件内容
/// </summary>
public partial class FilePreviewWindow : Window
{
    private readonly string _filePath;

    /// <summary>
    /// 获取文件名
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// 获取或设置文件内容
    /// </summary>
    public string FileContent { get; set; }

    /// <summary>
    /// 初始化 FilePreviewWindow 的新实例
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public FilePreviewWindow(string filePath)
    {
        InitializeComponent();
        _filePath = filePath;
        FileName = Path.GetFileName(filePath);
        FileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
        DataContext = this;
    }

    /// <summary>
    /// 取消按钮点击事件
    /// </summary>
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    /// <summary>
    /// 保存按钮点击事件
    /// </summary>
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
