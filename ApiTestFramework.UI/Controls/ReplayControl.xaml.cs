using ApiTestFramework.UI.Models;
using ApiTestFramework.UI.ViewModels;
using ApiTestFramework.UI.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ApiTestFramework.UI.Controls;

public partial class ReplayControl : UserControl
{
    public ReplayControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 双击复现结果行，弹出对应的录制请求详情窗口
    /// </summary>
    private void ResultList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ResultListView.SelectedItem is ReplayResultItem result &&
            DataContext is ReplayViewModel viewModel)
        {
            var request = viewModel.FindRequest(result.Order);
            if (request == null) return;

            var window = new CapturedRequestDetailWindow
            {
                DataContext = CapturedRequestItem.FromDomain(request),
                Owner = Window.GetWindow(this)
            };
            window.ShowDialog();
        }
    }
}
