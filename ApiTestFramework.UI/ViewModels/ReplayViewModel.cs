using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Domain.Enums;
using ApiTestFramework.UI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ApiTestFramework.UI.ViewModels;

/// <summary>
/// 录制会话复现视图模型，按录制顺序重新执行已捕获的请求
/// </summary>
public partial class ReplayViewModel : ObservableObject
{
    private readonly IReplayService _replayService;

    /// <summary>
    /// 当前加载的录制会话
    /// </summary>
    private RecordedSession? _currentSession;

    /// <summary>
    /// 会话名称
    /// </summary>
    [ObservableProperty]
    private string _sessionName = string.Empty;

    /// <summary>
    /// 复现汇总信息
    /// </summary>
    [ObservableProperty]
    private string _summary = "尚未执行复现";

    /// <summary>
    /// 是否正在执行复现
    /// </summary>
    [ObservableProperty]
    private bool _isExecuting;

    /// <summary>
    /// 复现结果列表
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ReplayResultItem> _results = [];

    public ReplayViewModel(IReplayService replayService)
    {
        _replayService = replayService;
    }

    /// <summary>
    /// 加载请求文件夹：将文件夹内的请求按现有顺序构建为待复现会话
    /// </summary>
    /// <param name="folder">请求文件夹节点</param>
    public void LoadFolder(RequestFolder folder)
    {
        var session = new RecordedSession { Name = folder.Name };
        var order = 1;

        foreach (var child in folder.Children.OfType<RequestItemNode>())
        {
            session.Requests.Add(new RecordedHttpRequest
            {
                Order = order++,
                Method = ToHttpMethod(child.RequestVerb),
                Url = child.Path,
                Headers = child.Headers.ToDictionary(h => h.Key, h => h.Value),
                Body = child.Body
            });
        }

        _currentSession = session;
        SessionName = folder.Name;
        Results.Clear();
        Summary = session.Requests.Count == 0
            ? "文件夹中没有请求"
            : $"共 {session.Requests.Count} 个请求，尚未执行复现";
    }

    /// <summary>
    /// 将请求动词枚举转换为 HTTP 方法字符串
    /// </summary>
    private static string ToHttpMethod(RequestVerbEnum verb) => verb switch
    {
        RequestVerbEnum.Get => "GET",
        RequestVerbEnum.Post => "POST",
        RequestVerbEnum.Put => "PUT",
        RequestVerbEnum.Delete => "DELETE",
        RequestVerbEnum.Patch => "PATCH",
        _ => "GET"
    };

    /// <summary>
    /// 按执行顺序查找当前会话中对应的录制请求
    /// </summary>
    /// <param name="order">请求顺序号</param>
    /// <returns>录制的请求，未找到时返回 null</returns>
    public RecordedHttpRequest? FindRequest(int order)
    {
        return _currentSession?.Requests.FirstOrDefault(r => r.Order == order);
    }

    /// <summary>
    /// 执行复现：按录制顺序发送请求并逐条刷新结果
    /// </summary>
    [RelayCommand]
    private async Task ExecuteReplay()
    {
        if (_currentSession == null || IsExecuting) return;

        IsExecuting = true;
        Results.Clear();

        var total = 0;
        var success = 0;
        var fail = 0;

        try
        {
            await _replayService.ExecuteAsync(_currentSession, r =>
            {
                total++;
                if (r.Success)
                {
                    success++;
                }
                else
                {
                    fail++;
                }

                Results.Add(ReplayResultItem.FromDomain(r));
            });

            Summary = $"总计 {total} 个请求：成功 {success} 个，失败 {fail} 个";
        }
        catch (Exception ex)
        {
            Summary = $"复现执行出错：{ex.Message}";
        }
        finally
        {
            IsExecuting = false;
        }
    }
}
