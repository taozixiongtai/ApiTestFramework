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
/// </list>
/// </remarks>
public partial class SeedDataDetailViewModel : ObservableObject
{
    private readonly ISeedDataService _seedDataService;
    private SeedDataNode? _currentNode;

    /// <summary>
    /// 种子文件列表
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> _seedFiles = new();

    /// <summary>
    /// 当前选中的文件名
    /// </summary>
    [ObservableProperty]
    private string? _selectedFile;

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
    /// 加载种子文件列表
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    [RelayCommand]
    private async Task LoadFiles()
    {
        var files = await _seedDataService.GetSeedFilesAsync();
        SeedFiles.Clear();
        foreach (var file in files)
        {
            SeedFiles.Add(file);
        }
    }

    /// <summary>
    /// 上传 JSON 文件
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    [RelayCommand]
    private async Task UploadFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "JSON 文件 (*.json)|*.json",
            Multiselect = true,
            Title = "选择种子数据文件"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            foreach (var fileName in openFileDialog.FileNames)
            {
                var content = await File.ReadAllTextAsync(fileName);
                var destFileName = Path.GetFileName(fileName);
                await _seedDataService.SaveSeedFileAsync(destFileName, content);
            }

            await LoadFiles();
            MessageBox.Show("文件上传成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    /// <summary>
    /// 保存当前编辑的文件
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    [RelayCommand]
    private async Task SaveFile()
    {
        if (string.IsNullOrEmpty(SelectedFile))
        {
            MessageBox.Show("请先选择一个文件", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await _seedDataService.SaveSeedFileAsync(SelectedFile, FileContent);
        MessageBox.Show("文件保存成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// 执行种子数据插入
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    [RelayCommand]
    private async Task Execute()
    {
        IsExecuting = true;
        ResultMessage = string.Empty;

        try
        {
            if (!string.IsNullOrWhiteSpace(FileContent) && !string.IsNullOrEmpty(SelectedFile))
            {
                await _seedDataService.SaveSeedFileAsync(SelectedFile, FileContent);
            }

            await _seedDataService.ExecuteSeedDataAsync();
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
    /// 删除选中的文件
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    [RelayCommand]
    private async Task DeleteFile()
    {
        if (string.IsNullOrEmpty(SelectedFile))
        {
            MessageBox.Show("请先选择一个文件", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"确定要删除文件 '{SelectedFile}' 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            await _seedDataService.DeleteSeedFileAsync(SelectedFile);
            FileContent = string.Empty;
            SelectedFile = null;
            await LoadFiles();
        }
    }

    /// <summary>
    /// 当选中文件改变时，加载文件内容
    /// </summary>
    /// <param name="value">新选中的文件名</param>
    partial void OnSelectedFileChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            LoadFileContentAsync(value);
        }
    }

    /// <summary>
    /// 异步加载文件内容
    /// </summary>
    /// <param name="fileName">文件名</param>
    private async void LoadFileContentAsync(string fileName)
    {
        var content = await _seedDataService.GetFileContentAsync(fileName);
        FileContent = content;
    }

    /// <summary>
    /// 加载种子数据节点到视图
    /// </summary>
    /// <param name="node">种子数据节点</param>
    public void LoadSeedData(SeedDataNode node)
    {
        _currentNode = node;
        _ = LoadFiles();
        if (!string.IsNullOrEmpty(node.FileName))
        {
            SelectedFile = node.FileName;
        }
        FileContent = node.Content;
    }

    /// <summary>
    /// 清空当前视图状态
    /// </summary>
    public void Clear()
    {
        _currentNode = null;
        SeedFiles.Clear();
        SelectedFile = null;
        FileContent = string.Empty;
        ResultMessage = string.Empty;
    }

    /// <summary>
    /// 将视图数据同步回节点
    /// </summary>
    public void SyncToNode()
    {
        if (_currentNode == null)
        {
            return;
        }

        _currentNode.FileName = SelectedFile ?? string.Empty;
        _currentNode.Content = FileContent;
    }
}
