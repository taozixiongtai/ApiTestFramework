using ApiTestFramework.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace ApiTestFramework.UI.Models;

public partial class SeedDataNode : RequestNode
{
    public SeedDataNode()
    {
        NodeType = TreeNodeTypeEnum.Seed;
    }

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private bool _fileExists = true;

    public void CheckFileExists()
    {
        FileExists = File.Exists(FilePath);
    }
}
