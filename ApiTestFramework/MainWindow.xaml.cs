using ApiTestFramework.Models;
using ApiTestFramework.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ApiTestFramework;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (DataContext is MainViewModel viewModel && e.NewValue is RequestNode node)
        {
            viewModel.TreeViewModel.OnNodeSelected(node);
        }
    }

    private void TreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel) return;

        viewModel.TreeViewModel.UpdateContextMenuItems();

        TreeContextMenu.Items.Clear();

        if (viewModel.TreeViewModel.ContextMenuItems.Count == 0)
        {
            e.Handled = true;
            return;
        }

        foreach (var item in viewModel.TreeViewModel.ContextMenuItems)
        {
            var menuItem = new MenuItem
            {
                Header = item.Header,
                Command = item.Command
            };
            TreeContextMenu.Items.Add(menuItem);
        }
    }

    private void AddHeader_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.DetailViewModel.AddHeader();
        }
    }

    private void DeleteHeader_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is KeyValuePair<string, string> header)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.DetailViewModel.RemoveHeader(header);
            }
        }
    }

    private void AddVariable_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.SettingsViewModel.AddVariable();
        }
    }

    private void DeleteVariable_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is KeyValuePair<string, string> variable)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SettingsViewModel.RemoveVariable(variable);
            }
        }
    }
}
