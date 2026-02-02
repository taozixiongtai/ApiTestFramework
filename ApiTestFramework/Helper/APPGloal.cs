using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTestFramework.Helper;

public static class APPGloal
{

    public static Dictionary<string, string> Dic { set; get; } = [];

    public static string BaseUrl { set; get; } = string.Empty;
}
