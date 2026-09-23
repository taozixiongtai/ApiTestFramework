using System.ComponentModel;

namespace ApiTestFramework.Domain.Enums;

public enum TreeNodeTypeEnum
{
    [Description("文件夹")]
    Folder = 0,

    [Description("请求")]
    Request = 1,

    /// <summary>
    /// 种子数据节点（功能已移除，仅为兼容旧持久化 JSON 中的 nodeType 数值保留，新数据不应再使用）
    /// </summary>
    [Description("种子数据")]
    Seed = 2,

    [Description("Web 录制")]
    WebRecorder = 3,

    [Description("录制会话")]
    RecordingSession = 4,
}
