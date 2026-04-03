using ApiTestFramework.UI.Messages;
using ApiTestFramework.UI.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;

namespace ApiTestFramework.UI.Views;

public partial class FilePreviewWindow : Window
{
    public FilePreviewWindow(string filePath)
    {
        InitializeComponent();
        DataContext = new FilePreviewViewModel(filePath);

        WeakReferenceMessenger.Default.Register<FileSavedMessage>(this, (_, _) =>
        {
            DialogResult = true;
            Close();
        });

        WeakReferenceMessenger.Default.Register<FileCancelledMessage>(this, (_, _) =>
        {
            DialogResult = false;
            Close();
        });
    }
}