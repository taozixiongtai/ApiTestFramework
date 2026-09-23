using ApiTestFramework.Domain.Entities;
using ApiTestFramework.UI.Models;
using Microsoft.Web.WebView2.Core;
using System.Collections.ObjectModel;

namespace ApiTestFramework.UI.Infrastructure;

/// <summary>
/// WebView2 请求捕获服务接口，用于 Web 录制功能
/// </summary>
public interface IWebRequestCaptureService
{
    /// <summary>
    /// 已捕获的请求集合（按捕获顺序）
    /// </summary>
    ObservableCollection<CapturedRequestItem> CapturedItems { get; }

    /// <summary>
    /// 附加到指定的 WebView2 实例并开始捕获（幂等）
    /// </summary>
    /// <param name="coreWebView2">要捕获的 WebView2 核心</param>
    void Attach(CoreWebView2 coreWebView2);

    /// <summary>
    /// 清空已捕获的请求与待匹配映射
    /// </summary>
    void Clear();

    /// <summary>
    /// 将已捕获的请求构建为录制会话
    /// </summary>
    /// <param name="name">会话名称</param>
    /// <returns>录制会话实体</returns>
    RecordedSession BuildSession(string name);
}
