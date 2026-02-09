using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTestFramework.Infrastructure.APP;

/// <summary>
/// 用于保存全局数据
/// </summary>
public static class APPGloal
{
    /// <summary>
    /// 请求变量
    /// </summary>
    public static Dictionary<string, string> RequestVariable { set; get; } = [];

}
