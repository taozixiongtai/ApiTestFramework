using System.ComponentModel;

namespace ApiTestFramework.Domain.Enums;

public enum RequestVerbEnum
{
    [Description("GET")]
    Get = 1,

    [Description("POST")]
    Post = 2,

    [Description("PUT")]
    Put = 3,

    [Description("DELETE")]
    Delete = 4,

    [Description("PATCH")]
    Patch = 5,
}
