using ApiTestFramework.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTestFramework.Infrastructure.Domain
{
    /// <summary>
    /// 请求项
    /// </summary>
    public class RequestItem
    {
        /// <summary>
        /// 请求动词
        /// </summary>
        public RequestVerbEnum RequestVerb { set; get; } = RequestVerbEnum.None;

        /// <summary>
        /// 请求路径
        /// </summary>
        public string Path { set; get; } = string.Empty;

        /// <summary>
        ///  请求体
        /// </summary>
        public string Body { set; get; } = string.Empty;

        /// <summary>
        /// 请求头
        /// 会和全局请求头进行合并，全局请求头优先级更高
        /// </summary>
        public Dictionary<string, string> Header { set; get; } = [];
    }
}
