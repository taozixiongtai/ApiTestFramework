using ApiTestFramework.Infrastructure.Enum;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ApiTestFramework.Models;

/// <summary>
/// 树节点基类，表示树形结构中的一个节点
/// </summary>
/// <remarks>
/// <para>这是树形结构中所有节点的基类，包括文件夹节点和请求节点。</para>
/// <para>使用 CommunityToolkit.Mvvm 的 [ObservableProperty] 特性实现属性变更通知。</para>
/// <para>继承关系：</para>
/// <list type="bullet">
///   <item><description>RequestNode (基类)</description></item>
///   <item><description>├── RequestFolder (文件夹节点)</description></item>
///   <item><description>└── RequestItemNode (请求节点)</description></item>
/// </list>
/// </remarks>
public partial class RequestNode : ObservableObject
{
    /// <summary>
    /// 节点唯一标识符
    /// </summary>
    /// <remarks>
    /// 使用 GUID 自动生成，用于持久化和节点查找
    /// </remarks>
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    /// <summary>
    /// 节点显示名称
    /// </summary>
    /// <remarks>
    /// 在树形结构中显示的文本，用户可自定义
    /// </remarks>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// 父节点标识符
    /// </summary>
    /// <remarks>
    /// 用于建立节点间的父子关系，根级别节点的 ParentId 为 null
    /// </remarks>
    [ObservableProperty]
    private string? _parentId;

    /// <summary>
    /// 节点类型
    /// </summary>
    /// <remarks>
    /// 区分节点是文件夹还是请求，影响 UI 显示和操作行为
    /// </remarks>
    [ObservableProperty]
    private TreeNodeTypeEnum _nodeType;

    /// <summary>
    /// 节点是否处于展开状态
    /// </summary>
    /// <remarks>
    /// 仅对文件夹节点有效，控制树形结构中子节点的显示/隐藏
    /// </remarks>
    [ObservableProperty]
    private bool _isExpanded;

    /// <summary>
    /// 节点是否处于选中状态
    /// </summary>
    /// <remarks>
    /// 用于高亮当前选中的节点，支持双向绑定
    /// </remarks>
    [ObservableProperty]
    private bool _isSelected;
}
