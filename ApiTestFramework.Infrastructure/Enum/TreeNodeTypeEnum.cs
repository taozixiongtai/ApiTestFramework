using System.ComponentModel;

namespace ApiTestFramework.Infrastructure.Enum
{
    public enum TreeNodeTypeEnum
    {
        /// <summary>
        /// 文件夹
        /// </summary>
        [Description("文件夹")]
        Folder = 0,

        /// <summary>
        /// 请求
        /// </summary>
        [Description("请求")]
        Request = 1,
    }
}
