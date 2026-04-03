using ApiTestFramework.UI.Models;
using ApiTestFramework.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using ApiTestFramework.UI.Views;

namespace ApiTestFramework.UI.ViewModels;

public partial class SeedDataDetailViewModel : ObservableObject
{
    private readonly ISeedDataService _seedDataService;
    private SeedDataNode? _currentNode;

    [ObservableProperty]
    private ObservableCollection<SeedFileItem> _seedFiles = new();

    [ObservableProperty]
    private bool _isExecuting;

    [ObservableProperty]
    private string _resultMessage = string.Empty;

    public SeedDataDetailViewModel(ISeedDataService seedDataService)
    {
        _seedDataService = seedDataService;
    }

    [RelayCommand]
    private void AddFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "JSON 文件 (*.json)|*.json",
            Title = "选择种子数据文件",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() == true && openFileDialog.FileNames != null)
        {
            foreach (var filePath in openFileDialog.FileNames)
            {
                var fileItem = SeedFileItem.FromFile(filePath);
                SeedFiles.Add(fileItem);
            }
        }
    }

    [RelayCommand]
    private void DeleteFile(SeedFileItem fileItem)
    {
        SeedFiles.Remove(fileItem);
    }

    [RelayCommand]
    private void EditFile(SeedFileItem fileItem)
    {
        var previewWindow = new FilePreviewWindow(fileItem.FilePath);


        previewWindow.ShowDialog();
    }

    [RelayCommand]
    private async Task ExecuteAll()
    {
        if (SeedFiles.Count == 0)
        {
            MessageBox.Show("请先添加文件", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsExecuting = true;
        ResultMessage = string.Empty;

        try
        {
            var filePaths = SeedFiles.Select(f => f.FilePath).ToArray();
            await _seedDataService.ExecuteSeedDataAsync(filePaths);
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

    public void LoadSeedData(SeedDataNode node)
    {
        _currentNode = node;
        node.CheckFileExists();
        LoadFiles();
    }

    public void Clear()
    {
        _currentNode = null;
        SeedFiles.Clear();
        ResultMessage = string.Empty;
    }

    public void SyncToNode()
    {
    }

    private void LoadFiles()
    {
        SeedFiles.Clear();
        if (_currentNode != null && !string.IsNullOrEmpty(_currentNode.FilePath))
        {
            var directory = Path.GetDirectoryName(_currentNode.FilePath);
            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                var files = Directory.GetFiles(directory, "*.json");
                foreach (var file in files)
                {
                    SeedFiles.Add(SeedFileItem.FromFile(file));
                }
            }
        }
    }
}
