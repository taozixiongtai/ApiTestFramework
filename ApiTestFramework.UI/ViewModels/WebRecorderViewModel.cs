using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.Domain.Entities;
using ApiTestFramework.UI.Infrastructure;
using ApiTestFramework.UI.Messages;
using ApiTestFramework.UI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Collections.ObjectModel;
using System.Windows;

namespace ApiTestFramework.UI.ViewModels;

/// <summary>
/// Web 录制视图模型，驱动 WebView2 浏览器导航与请求捕获
/// </summary>
public partial class WebRecorderViewModel : ObservableObject
{
    private readonly IWebRequestCaptureService _captureService;
    private readonly IRepository<RecordedSessionCollection> _sessionRepository;

    /// <summary>
    /// 浏览器控件引用
    /// </summary>
    private WebView2? _webView;

    /// <summary>
    /// 地址栏内容
    /// </summary>
    [ObservableProperty]
    private string _address = string.Empty;

    /// <summary>
    /// 会话名称
    /// </summary>
    [ObservableProperty]
    private string _sessionName = DefaultSessionName();

    public WebRecorderViewModel(IWebRequestCaptureService captureService, IRepository<RecordedSessionCollection> sessionRepository)
    {
        _captureService = captureService;
        _sessionRepository = sessionRepository;
    }

    /// <summary>
    /// 已捕获的请求集合（来自捕获服务）
    /// </summary>
    public ObservableCollection<CapturedRequestItem> CapturedItems => _captureService.CapturedItems;

    /// <summary>
    /// 附加浏览器控件：保存引用并开始捕获
    /// </summary>
    /// <param name="webView">浏览器控件</param>
    public void AttachBrowser(WebView2 webView)
    {
        if (_webView == webView) return;

        _webView = webView;
        _captureService.Attach(webView.CoreWebView2!);
        webView.NavigationCompleted += OnNavigationCompleted;
    }

    /// <summary>
    /// 导航完成后同步地址栏内容
    /// </summary>
    private void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (_webView == null) return;

        Address = _webView.Source?.ToString() ?? Address;
    }

    /// <summary>
    /// 生成默认会话名（"录制会话 " + 时间戳）
    /// </summary>
    private static string DefaultSessionName() => "录制会话 " + DateTime.Now.ToString("yyyyMMdd-HHmmss");

    /// <summary>
    /// 导航到地址栏内容（无协议前缀时补全 https://）
    /// </summary>
    [RelayCommand]
    private void Navigate()
    {
        if (string.IsNullOrWhiteSpace(Address)) return;

        var target = Address.Trim();
        if (!target.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !target.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            target = "https://" + target;
        }

        _webView?.CoreWebView2?.Navigate(target);
    }

    /// <summary>
    /// 后退
    /// </summary>
    [RelayCommand]
    private void Back()
    {
        _webView?.GoBack();
    }

    /// <summary>
    /// 前进
    /// </summary>
    [RelayCommand]
    private void Forward()
    {
        _webView?.GoForward();
    }

    /// <summary>
    /// 刷新
    /// </summary>
    [RelayCommand]
    private void Refresh()
    {
        _webView?.Reload();
    }

    /// <summary>
    /// 保存录制会话到本地，并通知左侧树刷新
    /// </summary>
    [RelayCommand]
    private async Task Save()
    {
        if (CapturedItems.Count == 0) return;

        var session = _captureService.BuildSession(SessionName);
        var list = await _sessionRepository.GetAsync();
        list.Add(session);
        await _sessionRepository.SaveAsync(list);

        WeakReferenceMessenger.Default.Send(new RecordingSessionSavedMessage());
        MessageBox.Show("录制会话已保存，可在左侧树\"Web 录制\"节点下查看", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

        _captureService.Clear();
        SessionName = DefaultSessionName();
    }

    /// <summary>
    /// 清空已捕获的请求
    /// </summary>
    [RelayCommand]
    private void Clear()
    {
        _captureService.Clear();
        SessionName = DefaultSessionName();
    }
}
