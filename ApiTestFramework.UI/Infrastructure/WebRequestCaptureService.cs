using ApiTestFramework.Domain.Entities;
using ApiTestFramework.UI.Models;
using Microsoft.Web.WebView2.Core;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace ApiTestFramework.UI.Infrastructure;

/// <summary>
/// WebView2 请求捕获服务，通过 WebResourceRequested/WebResourceResponseReceived 事件捕获 fetch/XHR 请求
/// </summary>
public class WebRequestCaptureService : IWebRequestCaptureService
{
    /// <summary>
    /// 请求对象到捕获条目的映射，用于响应到达时回填状态码
    /// </summary>
    private readonly Dictionary<CoreWebView2WebResourceRequest, CapturedRequestItem> _pendingRequests = [];

    /// <summary>
    /// 已附加的 WebView2 核心（幂等标记）
    /// </summary>
    private CoreWebView2? _attachedCoreWebView2;

    /// <summary>
    /// 已捕获的请求集合（按捕获顺序）
    /// </summary>
    public ObservableCollection<CapturedRequestItem> CapturedItems { get; } = [];

    /// <summary>
    /// 附加到指定的 WebView2 实例并开始捕获（重复附加同一实例时直接返回）
    /// </summary>
    /// <param name="coreWebView2">要捕获的 WebView2 核心</param>
    public void Attach(CoreWebView2 coreWebView2)
    {
        if (_attachedCoreWebView2 == coreWebView2) return;

        _attachedCoreWebView2 = coreWebView2;
        coreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Fetch);
        coreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.XmlHttpRequest);
        coreWebView2.WebResourceRequested += OnWebResourceRequested;
        coreWebView2.WebResourceResponseReceived += OnWebResourceResponseReceived;
    }

    /// <summary>
    /// 处理 Web 资源请求事件：读取方法/URL/请求头/请求体并记录捕获条目
    /// </summary>
    private void OnWebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        var headers = new Dictionary<string, string>();
        foreach (var header in e.Request.Headers)
        {
            headers[header.Key] = header.Value;
        }

        // 请求体流必须复制读取后回写，避免消费原流导致请求失败
        string body = string.Empty;
        if (e.Request.Content != null)
        {
            var memoryStream = new MemoryStream();
            e.Request.Content.CopyTo(memoryStream);
            body = Encoding.UTF8.GetString(memoryStream.ToArray());
            memoryStream.Position = 0;
            e.Request.Content = memoryStream;
        }

        var item = new CapturedRequestItem
        {
            Order = CapturedItems.Count + 1,
            Method = e.Request.Method,
            Url = e.Request.Uri,
            StatusCode = 0,
            Body = body,
            Headers = new ObservableCollection<KeyValuePair<string, string>>(headers),
            Timestamp = DateTime.Now
        };

        CapturedItems.Add(item);
        _pendingRequests[e.Request] = item;
    }

    /// <summary>
    /// 处理响应到达事件：回填对应捕获条目的状态码（该事件对所有资源触发，仅处理映射中的请求）
    /// </summary>
    private void OnWebResourceResponseReceived(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
    {
        if (_pendingRequests.TryGetValue(e.Request, out var item))
        {
            item.StatusCode = e.Response.StatusCode;
            _pendingRequests.Remove(e.Request);
            return;
        }

        // 引用不匹配时退化为按 URL+方法匹配最早的未完成项
        CoreWebView2WebResourceRequest? matchedKey = null;
        CapturedRequestItem? matchedItem = null;
        foreach (var pair in _pendingRequests)
        {
            if (pair.Value.StatusCode == 0 &&
                string.Equals(pair.Value.Url, e.Request.Uri, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(pair.Value.Method, e.Request.Method, StringComparison.OrdinalIgnoreCase))
            {
                matchedKey = pair.Key;
                matchedItem = pair.Value;
                break;
            }
        }

        if (matchedItem != null && matchedKey != null)
        {
            matchedItem.StatusCode = e.Response.StatusCode;
            _pendingRequests.Remove(matchedKey);
        }
    }

    /// <summary>
    /// 清空已捕获的请求与待匹配映射
    /// </summary>
    public void Clear()
    {
        CapturedItems.Clear();
        _pendingRequests.Clear();
    }

    /// <summary>
    /// 将已捕获的请求构建为录制会话
    /// </summary>
    /// <param name="name">会话名称</param>
    /// <returns>录制会话实体</returns>
    public RecordedSession BuildSession(string name)
    {
        var session = new RecordedSession { Name = name };

        foreach (var item in CapturedItems)
        {
            session.Requests.Add(new RecordedHttpRequest
            {
                Order = item.Order,
                Method = item.Method,
                Url = item.Url,
                Headers = item.Headers.ToDictionary(h => h.Key, h => h.Value),
                Body = item.Body,
                StatusCode = item.StatusCode,
                Timestamp = item.Timestamp
            });
        }

        return session;
    }
}
