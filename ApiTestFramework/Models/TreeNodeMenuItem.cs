using ApiTestFramework.Infrastructure.Enum;

namespace ApiTestFramework.Models;

public class TreeNodeMenuItem
{
    public TreeNodeMenuActionEnum Action { get; set; }
    public string Header { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
