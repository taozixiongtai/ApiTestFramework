using ApiTestFramework.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ApiTestFramework;

/// <summary>
/// 设置窗口，用于管理全局配置
/// </summary>
public partial class SettingsWindow : Window
{
    /// <summary>
    /// 初始化设置窗口的新实例
    /// </summary>
    public SettingsWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 添加全局变量
    /// </summary>
    private void AddVariable_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel)
        {
            viewModel.AddVariable();
        }
    }

    /// <summary>
    /// 删除全局变量
    /// </summary>
    private void DeleteVariable_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is KeyValuePair<string, string> variable)
        {
            if (DataContext is SettingsViewModel viewModel)
            {
                viewModel.RemoveVariable(variable);
            }
        }
    }

    /// <summary>
    /// 添加全局请求头
    /// </summary>
    private void AddHeader_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel)
        {
            viewModel.AddHeader();
        }
    }

    /// <summary>
    /// 删除全局请求头
    /// </summary>
    private void DeleteHeader_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is KeyValuePair<string, string> header)
        {
            if (DataContext is SettingsViewModel viewModel)
            {
                viewModel.RemoveHeader(header);
            }
        }
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
        if (DataContext is SettingsViewModel viewModel)
        {
            await viewModel.SaveAsync();
            DialogResult = true;
            Close();
        }
    }
}
