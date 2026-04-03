using ApiTestFramework.UI.ViewModels;
using System.Windows;

namespace ApiTestFramework.UI.Views;

public partial class SettingsWindow : Window
{
    private SettingsViewModel? _viewModel;

    public SettingsWindow()
    {
        InitializeComponent();
    }

    public void SetViewModel(SettingsViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.Saved += () =>
        {
            DialogResult = true;
            Close();
        };
        _viewModel.Cancelled += () =>
        {
            DialogResult = false;
            Close();
        };
        DataContext = _viewModel;
    }
}
