using System.Windows.Input;

namespace ApiTestFramework.Models;

public class TreeNodeMenuItem
{
    public string Header { get; set; } = string.Empty;
    public ICommand? Command { get; set; }
}
