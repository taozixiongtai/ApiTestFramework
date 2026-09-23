using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.Domain.Entities;
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
    /// 加载录制会话节点
    /// </summary>
    /// <param name="node">录制会话节点</param>
    public void LoadSession(RecordingSessionNode node)
    {
        _currentSession = node.Session;
        SessionName = node.Session.Name;
        Results.Clear();
        Summary = $"共 {node.Session.Requests.Count} 个请求，尚未执行复现";
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
