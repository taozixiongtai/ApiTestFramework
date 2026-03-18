using System.ComponentModel;

namespace ApiTestFramework.Infrastructure.Enum;

public enum RequestVerbEnum
{
    /// <summary>
    /// GET 
    /// </summary>
    [Description("GET")]
    Get = 1,

    /// <summary>
    /// POST 
    /// </summary>
    [Description("POST")]
    Post = 2,

    /// <summary>
    /// PUT 
    /// </summary>
    [Description("PUT")]
    Put = 3,

    /// <summary>
    /// DELETE 
    /// </summary>
    [Description("DELETE")]
    Delete = 4,

    /// <summary>
    /// PATCH 
    /// </summary>
    [Description("PATCH")]
    Patch = 5,
}
