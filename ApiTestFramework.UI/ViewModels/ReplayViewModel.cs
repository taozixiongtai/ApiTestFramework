using ApiTestFramework.Application.Interfaces;
using ApiTestFramework.Domain.Models;
using ApiTestFramework.UI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ApiTestFramework.UI.ViewModels;

/// <summary>
/// 请求文件夹复现视图模型，按现有顺序重新执行文件夹内的请求
/// </summary>
public partial class ReplayViewModel : ObservableObject
{
    private readonly IReplayService _replayService;

    /// <summary>
    /// 当前加载的待复现请求列表
    /// </summary>
    private List<RequestItem>? _currentRequests;

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
    /// 加载请求文件夹：将文件夹内的请求按现有顺序构建为待复现列表
    /// </summary>
    /// <param name="folder">请求文件夹节点</param>
    public void LoadFolder(RequestFolder folder)
    {
        var requests = folder.Children.OfType<RequestItemNode>()
            .Select(ToRequestItem)
            .ToList();

        _currentRequests = requests;
        SessionName = folder.Name;
        Results.Clear();
        Summary = requests.Count == 0
            ? "文件夹中没有请求"
            : $"共 {requests.Count} 个请求，尚未执行复现";
    }

    /// <summary>
    /// 将请求树节点转换为领域请求实体
    /// </summary>
    private static RequestItem ToRequestItem(RequestItemNode node) => new()
    {
        RequestVerb = node.RequestVerb,
        Path = node.Path,
        Body = node.Body,
        Header = node.Headers.ToDictionary(h => h.Key, h => h.Value)
    };

    /// <summary>
    /// 执行复现：按顺序发送请求并逐条刷新结果
    /// </summary>
    [RelayCommand]
    private async Task ExecuteReplay()
    {
        if (_currentRequests == null || IsExecuting) return;

        IsExecuting = true;
        Results.Clear();

        var total = 0;
        var success = 0;
        var fail = 0;

        try
        {
            await _replayService.ExecuteAsync(_currentRequests, r =>
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
