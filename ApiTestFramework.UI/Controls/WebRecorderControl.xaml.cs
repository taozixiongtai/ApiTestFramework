using ApiTestFramework.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

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
}
