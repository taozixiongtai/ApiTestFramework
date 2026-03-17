using ApiTestFramework.Infrastructure.Domain;
using System.Collections.Generic;

namespace ApiTestFramework.Infrastructure.Domain
{
    /// <summary>
    /// 应用程序数据容器，包含所有需要持久化的数据
    /// </summary>
    public class AppData
    {
        /// <summary>
        /// 请求树根节点列表
        /// </summary>
        public List<RequestTreeItem> RequestTree { get; set; } = new();

        /// <summary>
        /// 全局设置
        /// </summary>
        public GlobalSettings Settings { get; set; } = new();
    }
}
