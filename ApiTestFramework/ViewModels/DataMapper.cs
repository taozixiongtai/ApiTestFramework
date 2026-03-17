using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Infrastructure.Enum;
using ApiTestFramework.Models;
using System.Collections.ObjectModel;

namespace ApiTestFramework.ViewModels;

public static class DataMapper
{
    public static ObservableCollection<RequestNode> ToViewModel(List<RequestTreeItem> items)
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

    public static RequestNode? ToViewModel(RequestTreeItem item)
    {
        if (item.NodeType == TreeNodeTypeEnum.Folder)
        {
            var folder = new RequestFolder
            {
                Id = item.Id,
                Name = item.Name,
                ParentId = item.ParentId,
                IsExpanded = item.IsExpanded
            };

            foreach (var child in item.Children)
            {
                var childNode = ToViewModel(child);
                if (childNode != null)
                {
                    folder.Children.Add(childNode);
                }
            }

            return folder;
        }
        else if (item.NodeType == TreeNodeTypeEnum.Request && item.RequestItem != null)
        {
            var request = new RequestItemNode
            {
                Id = item.Id,
                Name = item.Name,
                ParentId = item.ParentId,
                RequestVerb = item.RequestItem.RequestVerb,
                Path = item.RequestItem.Path,
                Body = item.RequestItem.Body
            };

            foreach (var header in item.RequestItem.Header)
            {
                request.Headers.Add(new KeyValuePair<string, string>(header.Key, header.Value));
            }

            return request;
        }

        return null;
    }

    public static List<RequestTreeItem> ToDomain(ObservableCollection<RequestNode> nodes)
    {
        var result = new List<RequestTreeItem>();

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

    public static RequestTreeItem? ToDomain(RequestNode node)
    {
        if (node is RequestFolder folder)
        {
            var item = new RequestTreeItem
            {
                Id = folder.Id,
                Name = folder.Name,
                ParentId = folder.ParentId,
                NodeType = TreeNodeTypeEnum.Folder,
                IsExpanded = folder.IsExpanded
            };

            foreach (var child in folder.Children)
            {
                var childItem = ToDomain(child);
                if (childItem != null)
                {
                    item.Children.Add(childItem);
                }
            }

            return item;
        }
        else if (node is RequestItemNode request)
        {
            var item = new RequestTreeItem
            {
                Id = request.Id,
                Name = request.Name,
                ParentId = request.ParentId,
                NodeType = TreeNodeTypeEnum.Request,
                RequestItem = new RequestItem
                {
                    RequestVerb = request.RequestVerb,
                    Path = request.Path,
                    Body = request.Body
                }
            };

            foreach (var header in request.Headers)
            {
                item.RequestItem.Header[header.Key] = header.Value;
            }

            return item;
        }

        return null;
    }
}
