using System.Collections.Generic;

namespace ApiTestFramework.Infrastructure.Domain
{
    /// <summary>
    /// 全局设置，用于持久化保存应用程序配置
    /// </summary>
    public class GlobalSettings
    {
        /// <summary>
        /// 认证Token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 全局请求变量
        /// </summary>
        public Dictionary<string, string> Variables { get; set; } = new();

        /// <summary>
        /// 全局请求头
        /// </summary>
        public Dictionary<string, string> GlobalHeaders { get; set; } = new();

        /// <summary>
        /// 基础URL
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;
    }
}
