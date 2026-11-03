using ApiTestFramework.Domain.Enums;
using ApiTestFramework.Domain.Models;
using ApiTestFramework.UI.Models;
using Mapster;
using System.Collections.ObjectModel;

namespace ApiTestFramework.UI.Mapper;

public static class DataMapper
{
    public static void Configure()
    {
        TypeAdapterConfig<RequestTreeNode, RequestFolder>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.NodeType = TreeNodeTypeEnum.Folder;
            });

        TypeAdapterConfig<RequestTreeNode, RequestItemNode>
            .NewConfig()
            .Map(dest => dest.RequestVerb, src => src.RequestItem != null ? src.RequestItem.RequestVerb : default)
            .Map(dest => dest.Path, src => src.RequestItem != null ? src.RequestItem.Path : null)
            .Map(dest => dest.Body, src => src.RequestItem != null ? src.RequestItem.Body : null)
            .AfterMapping((src, dest) =>
            {
                dest.NodeType = TreeNodeTypeEnum.Request;
                if (src.RequestItem != null)
                {
                    foreach (var header in src.RequestItem.Header)
                    {
                        dest.Headers.Add(new KeyValuePair<string, string>(header.Key, header.Value));
                    }
                }
            });

        TypeAdapterConfig<RequestFolder, RequestTreeNode>
            .NewConfig()
            .Map(dest => dest.NodeType, src => TreeNodeTypeEnum.Folder)
            .Map(dest => dest.RequestItem, src => (RequestItem?)null);

        TypeAdapterConfig<RequestItemNode, RequestTreeNode>
            .NewConfig()
            .Map(dest => dest.NodeType, src => TreeNodeTypeEnum.Request)
            .Map(dest => dest.IsExpanded, src => false)
            .Map(dest => dest.Children, src => new List<RequestTreeNode>())
            .AfterMapping((src, dest) =>
            {
                dest.RequestItem = new RequestItem
                {
                    RequestVerb = src.RequestVerb,
                    Path = src.Path,
                    Body = src.Body
                };
                foreach (var header in src.Headers)
                {
                    dest.RequestItem.Header[header.Key] = header.Value;
                }
            });

    }

    public static ObservableCollection<RequestNode> ToViewModel(List<RequestTreeNode> items)
    {
        var result = new ObservableCollection<RequestNode>();

        foreach (var item in items)
        {
            var node = ToViewModel(item);
            if (node != null)
            {
                result.Add(node);
            }
        }

        return result;
    }

    public static RequestNode? ToViewModel(RequestTreeNode item)
    {
        if (item.NodeType == TreeNodeTypeEnum.Folder)
        {
            return item.Adapt<RequestFolder>();
        }
        else if (item.NodeType == TreeNodeTypeEnum.Request && item.RequestItem != null)
        {
            return item.Adapt<RequestItemNode>();
        }

        return null;
    }

    public static List<RequestTreeNode> ToDomain(ObservableCollection<RequestNode> nodes)
    {
        var result = new List<RequestTreeNode>();

        foreach (var node in nodes)
        {
            var item = ToDomain(node);
            if (item != null)
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static RequestTreeNode? ToDomain(RequestNode node)
    {
        if (node is RequestFolder folder)
        {
            return folder.Adapt<RequestTreeNode>();
        }
        else if (node is RequestItemNode request)
        {
            return request.Adapt<RequestTreeNode>();
        }

        return null;
    }
}
