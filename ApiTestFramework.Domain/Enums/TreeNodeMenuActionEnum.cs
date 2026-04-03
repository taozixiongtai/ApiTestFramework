using System.ComponentModel;

namespace ApiTestFramework.Domain.Enums;

public enum TreeNodeMenuActionEnum
{
    [Description("删除")]
    Delete,

    [Description("重命名")]
    Rename,

    [Description("复制")]
    Duplicate,

    [Description("上移")]
    MoveUp,

    [Description("下移")]
    MoveDown
}
