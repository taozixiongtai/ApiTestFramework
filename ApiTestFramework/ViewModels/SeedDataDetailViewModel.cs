using ApiTestFramework.Models;
using ApiTestFramework.Service.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace ApiTestFramework.ViewModels;

/// <summary>
/// 种子数据详情视图模型，管理种子文件的上传、编辑和执行
/// </summary>
/// <remarks>
/// <para>该类负责管理种子数据的功能，包括：</para>
/// <list type="bullet">
///   <item><description>种子文件的上传、保存和删除</description></item>
///   <item><description>种子文件内容的编辑</description></item>
///   <item><description>执行种子数据插入到数据库</description></item>
///   <item><description>支持多文件上传和批量执行</description></item>
/// </list>
/// </remarks>
public partial class SeedDataDetailViewModel : ObservableObject
{
    private readonly ISeedDataService _seedDataService;

    /// <summary>
    /// 当前关联的种子数据节点
    /// </summary>
    private SeedDataNode? _currentNode;

    /// <summary>
    /// 当前种子数据节点
    /// </summary>
    public SeedDataNode? CurrentNode => _currentNode;

    /// <summary>
    /// 当前编辑的文件内容
    /// </summary>
    [ObservableProperty]
    private string _fileContent = string.Empty;

    /// <summary>
    /// 是否正在执行种子数据插入
    /// </summary>
    [ObservableProperty]
    private bool _isExecuting;

    /// <summary>
    /// 执行结果消息
    /// </summary>
    [ObservableProperty]
    private string _resultMessage = string.Empty;

    /// <summary>
    /// 初始化 SeedDataDetailViewModel 的新实例
    /// </summary>
    /// <param name="seedDataService">种子数据服务</param>
    public SeedDataDetailViewModel(ISeedDataService seedDataService)
    {
        _seedDataService = seedDataService;
    }

    /// <summary>
    /// 选择种子文件
    /// </summary>
    [RelayCommand]
    private void SelectFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "JSON 文件 (*.json)|*.json",
            Title = "选择种子数据文件"
        };

        if (openFileDialog.ShowDialog() == true && _currentNode != null)
        {
            _currentNode.FilePath = openFileDialog.FileName;
            _currentNode.FileName = Path.GetFileName(openFileDialog.FileName);
            _currentNode.CheckFileExists();
            LoadFileContent();
        }
    }

    /// <summary>
    /// 保存当前编辑的文件
    /// </summary>
    [RelayCommand]
    private async Task SaveFile()
    {
        if (_currentNode == null || string.IsNullOrEmpty(_currentNode.FilePath))
        {
            MessageBox.Show("请先选择一个文件", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!File.Exists(_currentNode.FilePath))
        {
            MessageBox.Show("文件不存在，无法保存", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        await File.WriteAllTextAsync(_currentNode.FilePath, FileContent);
        MessageBox.Show("文件保存成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// 执行种子数据插入
    /// </summary>
    [RelayCommand]
    private async Task Execute()
    {
        if (_currentNode == null || string.IsNullOrEmpty(_currentNode.FilePath))
        {
            MessageBox.Show("请先选择一个文件", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!File.Exists(_currentNode.FilePath))
        {
            MessageBox.Show("文件不存在，无法执行", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        IsExecuting = true;
        ResultMessage = string.Empty;

        try
        {
            if (!string.IsNullOrWhiteSpace(FileContent))
            {
                await File.WriteAllTextAsync(_currentNode.FilePath, FileContent);
            }

            await _seedDataService.ExecuteSeedDataAsync(new[] { _currentNode.FilePath });
            ResultMessage = "种子数据执行成功";
            MessageBox.Show("种子数据执行成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ResultMessage = $"执行失败: {ex.Message}";
            MessageBox.Show($"执行失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsExecuting = false;
        }
    }

    /// <summary>
    /// 预览当前文件
    /// </summary>
    [RelayCommand]
    private void PreviewFile()
    {
        if (_currentNode == null || string.IsNullOrEmpty(_currentNode.FilePath))
        {
            MessageBox.Show("请先选择一个文件", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var previewWindow = new FilePreviewWindow(_currentNode.FilePath)
        {
            Owner = Application.Current.MainWindow
        };

        if (previewWindow.ShowDialog() == true)
        {
            _currentNode.CheckFileExists();
            ResultMessage = $"文件 {_currentNode.FileName} 已保存";
        }
    }

    /// <summary>
    /// 加载文件内容
    /// </summary>
    private async void LoadFileContent()
    {
        if (_currentNode != null && File.Exists(_currentNode.FilePath))
        {
            FileContent = await File.ReadAllTextAsync(_currentNode.FilePath);
        }
        else
        {
            FileContent = string.Empty;
        }
    }

    /// <summary>
    /// 加载种子数据节点到视图
    /// </summary>
    /// <param name="node">种子数据节点</param>
    public void LoadSeedData(SeedDataNode node)
    {
        _currentNode = node;
        node.CheckFileExists();
        LoadFileContent();
    }

    /// <summary>
    /// 清空当前视图状态
    /// </summary>
    public void Clear()
    {
        _currentNode = null;
        FileContent = string.Empty;
        ResultMessage = string.Empty;
    }

    /// <summary>
    /// 将视图数据同步回节点
    /// </summary>
    public void SyncToNode()
    {
        // 数据已经存储在 _currentNode 中，无需额外同步
    }
}
