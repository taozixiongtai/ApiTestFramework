using ApiTestFramework.Domain.Entities;
using ApiTestFramework.UI.Models;
using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ApiTestFramework.UI.Views;

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
        if (sender is not TreeView { ContextMenu: ContextMenu menu }) return;

        viewModel.TreeViewModel.UpdateContextMenuItems();

        menu.Items.Clear();

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
            menu.Items.Add(menuItem);
        }
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow
        {
            Owner = this
        };

        var settingsRepository = App.AppHost?.Services.GetService(typeof(IRepository<GlobalSettings>));
        if (settingsRepository is IRepository<GlobalSettings> repository)
        {
            var settingsViewModel = new SettingsViewModel(repository);
            settingsWindow.SetViewModel(settingsViewModel);
        }

        if (settingsWindow.ShowDialog() == true)
        {
            MessageBox.Show("设置已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
