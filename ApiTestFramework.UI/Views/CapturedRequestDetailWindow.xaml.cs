using System.Windows;

namespace ApiTestFramework.UI.Views;

/// <summary>
/// 录制请求详情窗口，双击捕获列表或复现结果时弹出查看单个请求的完整信息
/// </summary>
public partial class CapturedRequestDetailWindow : Window
{
    public CapturedRequestDetailWindow()
    {
        InitializeComponent();
    }
}
