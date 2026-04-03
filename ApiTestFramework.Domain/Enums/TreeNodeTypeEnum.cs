using System.ComponentModel;

namespace ApiTestFramework.Domain.Enums;

public enum TreeNodeTypeEnum
{
    [Description("文件夹")]
    Folder = 0,

    [Description("请求")]
    Request = 1,

    [Description("种子数据")]
    Seed = 2,
}
