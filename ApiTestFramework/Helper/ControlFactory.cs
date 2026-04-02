using ApiTestFramework.Components;
using ApiTestFramework.Models;
using System.Windows.Controls;

namespace ApiTestFramework.Helper;

/// <summary>
/// 控件工厂类，负责根据节点类型创建对应的用户控件
/// </summary>
public static class ControlFactory
{
    /// <summary>
    /// 根据节点类型创建对应的用户控件
    /// </summary>
    /// <param name="node">节点对象</param>
    /// <returns>对应的用户控件</returns>
    public static UserControl CreateControl(RequestNode node)
    {
        return node switch
        {
            RequestItemNode => new RequestDetailControl(),
            _ => new EmptyControl()
        };
    }
}