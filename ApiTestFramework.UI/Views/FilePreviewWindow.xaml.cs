using ApiTestFramework.UI.ViewModels;
using System.Windows;

namespace ApiTestFramework.UI.Views;

public partial class FilePreviewWindow : Window
{
    private readonly FilePreviewViewModel _viewModel;

    public FilePreviewWindow(string filePath)
    {
        InitializeComponent();
        _viewModel = new FilePreviewViewModel(filePath);
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
