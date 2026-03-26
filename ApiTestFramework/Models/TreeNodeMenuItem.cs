using System.Windows.Input;

namespace ApiTestFramework.Models;

/// <summary>
/// 树节点右键菜单项，表示 ContextMenu 中的一个菜单项
/// </summary>
/// <remarks>
/// <para>用于动态生成树节点的右键菜单，支持命令绑定。</para>
/// <para>菜单项从 TreeNodeMenuActionEnum 枚举自动生成，显示文本从 [Description] 特性读取。</para>
/// </remarks>
public class TreeNodeMenuItem
{
    /// <summary>
    /// 菜单项显示文本
    /// </summary>
    /// <remarks>
    /// 从枚举的 [Description] 特性自动获取，如 "删除"、"重命名" 等
    /// </remarks>
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// 菜单项点击时执行的命令
    /// </summary>
    /// <remarks>
    /// 绑定到 ViewModel 中对应的 RelayCommand，如 DeleteNodeCommand、RenameNodeCommand 等
    /// </remarks>
    public ICommand? Command { get; set; }
}
