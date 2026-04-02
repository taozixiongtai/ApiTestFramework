using ApiTestFramework.Infrastructure.Enum;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ApiTestFramework.Models;

/// <summary>
/// 文件夹节点，用于组织和管理请求的树形结构
/// </summary>
/// <remarks>
/// <para>文件夹节点可以包含子节点（包括其他文件夹和请求节点），形成树形层次结构。</para>
/// <para>继承自 RequestNode，扩展了 Children 属性用于存储子节点集合。</para>
/// </remarks>
public partial class RequestFolder : RequestNode
{
    /// <summary>
    /// 初始化 RequestFolder 的新实例
    /// </summary>
    /// <remarks>
    /// 构造函数自动将 NodeType 设置为 Folder
    /// </remarks>
    public RequestFolder()
    {
        NodeType = TreeNodeTypeEnum.Folder;
    }

    /// <summary>
    /// 子节点集合
    /// </summary>
    /// <remarks>
    /// 可包含 RequestFolder（子文件夹）或 RequestItemNode（请求节点）
    /// </remarks>
    [ObservableProperty]
    private ObservableCollection<RequestNode> _children = new();
}

/// <summary>
/// 请求节点，表示一个具体的 HTTP 请求配置
/// </summary>
/// <remarks>
/// <para>请求节点存储完整的 HTTP 请求信息，包括请求方法、URL、请求头和请求体。</para>
/// <para>继承自 RequestNode，扩展了请求相关的属性。</para>
/// <para>当选中此类型节点时，右侧详情面板会显示请求的详细信息供用户编辑。</para>
/// </remarks>
public partial class RequestItemNode : RequestNode
{
    /// <summary>
    /// 初始化 RequestItemNode 的新实例
    /// </summary>
    /// <remarks>
    /// 构造函数自动将 NodeType 设置为 Request
    /// </remarks>
    public RequestItemNode()
    {
        NodeType = TreeNodeTypeEnum.Request;
    }

    /// <summary>
    /// HTTP 请求方法（GET、POST、PUT、DELETE、PATCH）
    /// </summary>
    [ObservableProperty]
    private RequestVerbEnum _requestVerb;

    /// <summary>
    /// 请求路径（URL）
    /// </summary>
    /// <remarks>
    /// 相对路径或绝对路径，如 /api/users 或 https://api.example.com/users
    /// </remarks>
    [ObservableProperty]
    private string _path = string.Empty;

    /// <summary>
    /// 请求体内容
    /// </summary>
    /// <remarks>
    /// 通常用于 POST、PUT、PATCH 请求，支持 JSON、XML 等格式
    /// </remarks>
    [ObservableProperty]
    private string _body = string.Empty;

    /// <summary>
    /// 请求头集合
    /// </summary>
    /// <remarks>
    /// 以键值对形式存储自定义请求头，如 Content-Type、Authorization 等
    /// </remarks>
    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _headers = new();

    [ObservableProperty]
    private string _response = string.Empty;

    [ObservableProperty]
    private int _statusCode;

    [ObservableProperty]
    private double _responseTime;

    [ObservableProperty]
    private bool _hasResponse;
}

/// <summary>
/// 种子数据节点，表示一个种子数据文件
/// </summary>
/// <remarks>
/// <para>种子数据节点存储种子文件的内容。</para>
/// <para>继承自 RequestNode，扩展了种子数据相关的属性。</para>
/// <para>当选中此类型节点时，右侧详情面板会显示种子数据的详细信息供用户编辑。</para>
/// </remarks>
public partial class SeedDataNode : RequestNode
{
    /// <summary>
    /// 初始化 SeedDataNode 的新实例
    /// </summary>
    /// <remarks>
    /// 构造函数自动将 NodeType 设置为 Seed
    /// </remarks>
    public SeedDataNode()
    {
        NodeType = TreeNodeTypeEnum.Seed;
    }

    /// <summary>
    /// 种子文件内容
    /// </summary>
    [ObservableProperty]
    private string _content = string.Empty;

    /// <summary>
    /// 文件名（包含扩展名）
    /// </summary>
    [ObservableProperty]
    private string _fileName = string.Empty;
}
