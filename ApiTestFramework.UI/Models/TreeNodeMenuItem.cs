using System.Windows.Input;

namespace ApiTestFramework.UI.Models;

public class TreeNodeMenuItem
{
    public string Header { get; set; } = string.Empty;

    public ICommand? Command { get; set; }
}
