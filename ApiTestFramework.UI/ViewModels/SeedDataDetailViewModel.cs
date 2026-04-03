using ApiTestFramework.UI.Models;
using ApiTestFramework.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using ApiTestFramework.UI.Views;

namespace ApiTestFramework.UI.ViewModels;

public partial class SeedDataDetailViewModel : ObservableObject
{
    private readonly ISeedDataService _seedDataService;

    private SeedDataNode? _currentNode;

    public SeedDataNode? CurrentNode => _currentNode;

    [ObservableProperty]
    private string _fileContent = string.Empty;

    [ObservableProperty]
    private bool _isExecuting;

    [ObservableProperty]
    private string _resultMessage = string.Empty;

    public SeedDataDetailViewModel(ISeedDataService seedDataService)
    {
        _seedDataService = seedDataService;
    }

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
            Owner = System.Windows.Application.Current.MainWindow
        };

        if (previewWindow.ShowDialog() == true)
        {
            _currentNode.CheckFileExists();
            ResultMessage = $"文件 {_currentNode.FileName} 已保存";
        }
    }

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

    public void LoadSeedData(SeedDataNode node)
    {
        _currentNode = node;
        node.CheckFileExists();
        LoadFileContent();
    }

    public void Clear()
    {
        _currentNode = null;
        FileContent = string.Empty;
        ResultMessage = string.Empty;
    }

    public void SyncToNode()
    {
    }
}
