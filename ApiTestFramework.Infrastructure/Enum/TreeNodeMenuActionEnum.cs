using System.ComponentModel;

namespace ApiTestFramework.Infrastructure.Enum;

public enum TreeNodeMenuActionEnum
{
    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete,

    /// <summary>
    /// 重命名
    /// </summary>
    [Description("重命名")]
    Rename,

    /// <summary>
    /// 复制
    /// </summary>
    [Description("复制")]
    Duplicate,

    /// <summary>
    /// 上移
    /// </summary>
    [Description("上移")]
    MoveUp,

    /// <summary>
    /// 下移
    /// </summary>
    [Description("下移")]
    MoveDown
}
