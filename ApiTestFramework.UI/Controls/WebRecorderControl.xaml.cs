using ApiTestFramework.UI.Models;
using ApiTestFramework.UI.ViewModels;
using ApiTestFramework.UI.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ApiTestFramework.UI.Controls;

public partial class WebRecorderControl : UserControl
{
    /// <summary>
    /// 防止重复初始化 WebView2 的标记
    /// </summary>
    private bool _initialized;

    public WebRecorderControl()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_initialized) return;
        _initialized = true;

        await WebBrowser.EnsureCoreWebView2Async();

        if (DataContext is WebRecorderViewModel vm)
        {
            vm.AttachBrowser(WebBrowser);
        }
    }

    /// <summary>
    /// 双击捕获条目弹出请求详情窗口
    /// </summary>
    private void CapturedList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (CapturedListView.SelectedItem is CapturedRequestItem item)
        {
            var window = new CapturedRequestDetailWindow
            {
                DataContext = item,
                Owner = Window.GetWindow(this)
            };
            window.ShowDialog();
        }
    }
}
