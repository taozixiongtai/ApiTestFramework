using ApiTestFramework.Domain.Entities;
using ApiTestFramework.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ApiTestFramework.UI.Models;

/// <summary>
/// Web 录制入口节点，作为所有录制会话的容器
/// </summary>
public partial class WebRecorderNode : RequestNode
{
    public WebRecorderNode()
    {
        NodeType = TreeNodeTypeEnum.WebRecorder;
        Name = "Web 录制";
        IsExpanded = true;
    }

    /// <summary>
    /// 录制会话子节点集合
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<RequestNode> _children = [];
}

/// <summary>
/// 录制会话节点，承载一个已保存的录制会话实体
/// </summary>
public partial class RecordingSessionNode : RequestNode
{
    public RecordingSessionNode()
    {
        NodeType = TreeNodeTypeEnum.RecordingSession;
        Name = Session.Name;
    }

    /// <summary>
    /// 关联的录制会话实体
    /// </summary>
    public RecordedSession Session { get; init; } = new();
}
