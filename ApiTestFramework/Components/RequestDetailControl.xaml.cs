using ApiTestFramework.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ApiTestFramework.Components;

public partial class RequestDetailControl : UserControl
{
    public RequestDetailControl()
    {
        InitializeComponent();
    }

    private void AddHeader_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is RequestDetailViewModel viewModel)
        {
            viewModel.AddHeader();
        }
    }

    private void DeleteHeader_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is System.Collections.Generic.KeyValuePair<string, string> header)
        {
            if (DataContext is RequestDetailViewModel viewModel)
            {
                viewModel.RemoveHeader(header);
            }
        }
    }
}