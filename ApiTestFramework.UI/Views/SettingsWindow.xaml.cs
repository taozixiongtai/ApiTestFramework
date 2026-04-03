using ApiTestFramework.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ApiTestFramework.UI.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void AddVariable_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel)
        {
            viewModel.AddVariable();
        }
    }

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

    private void AddHeader_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel)
        {
            viewModel.AddHeader();
        }
    }

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

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

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
