***

name: "code-conventions"
description: "定义ApiTestFramework项目的编码标准和规范。在编写、修改或审查代码时调用以确保一致性。"
-----------------------------------------------------------------

# ApiTestFramework 代码规范

本文档定义了 ApiTestFramework WPF 项目的编码标准和规范 - 这是一个类似 ApiFox 的 API 调试工具。

## 项目架构

### 解决方案结构

项目采用**三层架构**：

```
ApiTestFramework/
├── ApiTestFramework/                    # 主 WPF 应用程序
│   ├── Components/                      # UI 组件（异常处理器、工具类）
│   ├── Converters/                      # XAML 绑定的值转换器
│   ├── Mapper/                          # 对象映射配置
│   ├── Models/                          # 视图专用模型
│   ├── ViewModels/                      # ViewModel 类
│   └── Seed/                            # 种子数据
│
├── ApiTestFramework.Infrastructure/     # 基础设施层
│   ├── APP/                             # 应用程序配置和全局状态
│   ├── Domain/                          # 领域模型
│   ├── Enum/                            # 枚举定义
│   ├── Exceptions/                      # 自定义异常
│   ├── Extensions/                      # 扩展方法
│   ├── Helper/                          # 工具类
│   └── JsonTransform/                   # JSON 转换管道
│
└── ApiTestFramework.Service/            # 服务层
    ├── Interface/                       # 服务接口
    └── Services/                        # 服务实现
```

### 项目依赖关系

```
ApiTestFramework (主项目)
    └── ApiTestFramework.Service
            └── ApiTestFramework.Infrastructure
```

## 技术栈

- **框架**: .NET 10.0-windows (WPF)
- **MVVM 框架**: CommunityToolkit.Mvvm 8.4.0 (源生成器)
- **对象映射**: Mapster 7.4.0
- **依赖注入**: Microsoft.Extensions.DependencyInjection 10.0.2
- **配置管理**: Microsoft.Extensions.Configuration.Json 10.0.2
- **HTTP 客户端**: RestSharp 113.1.0
- **ORM**: SqlSugarCore 5.1.4.214

## 命名约定

### 类命名

| 类型         | 规范                    | 示例                                               |
| :--------- | :-------------------- | :----------------------------------------------- |
| ViewModel  | `{功能名}ViewModel`      | `MainViewModel`, `RequestTreeViewModel`          |
| Model      | `{实体名}` 或 `{实体名}Node` | `RequestNode`, `RequestFolder`, `GlobalSettings` |
| Service 接口 | `I{服务名}Service`       | `IDatabaseService`, `IRepository<T>`             |
| Service 实现 | `{服务名}Service`        | `DatabaseService`, `JsonRepository<T>`           |
| Converter  | `{转换逻辑}Converter`     | `BooleanToVisibilityConverter`                   |
| Enum       | `{功能名}Enum`           | `RequestVerbEnum`, `TreeNodeTypeEnum`            |
| Exception  | `{异常类型}Exception`     | `BusinessException`                              |
| Helper     | `{功能}Helper`          | `FileHelper`, `JsonHelper`                       |

### 属性和字段命名

```csharp
// 使用 ObservableProperty 的私有字段：下划线前缀
[ObservableProperty]
private string _name = string.Empty;

[ObservableProperty]
private bool _isExpanded;

// 自动生成的属性：PascalCase
// _name → Name
// _isExpanded → IsExpanded
```

### 方法命名

| 类型    | 规范           | 示例                                 |
| :---- | :----------- | :--------------------------------- |
| 公共方法  | `PascalCase` | `LoadRequest()`, `SyncToNode()`    |
| 私有方法  | `PascalCase` | `OnNodeSelected()`, `FindParent()` |
| 异步方法  | `{动作}Async`  | `SaveAsync()`, `SaveToDataAsync()` |
| 事件处理器 | `On{事件名}`    | `OnNodeSelected()`, `OnStartup()`  |
| 命令方法  | `{动作}`       | `AddFolder()`, `DeleteNode()`      |

### 命令命名

```csharp
// 方法名：AddFolder
// 自动生成的命令：AddFolderCommand
[RelayCommand]
private async Task AddFolder() { }
```

## MVVM 模式

### ViewModel 结构

```csharp
/// <summary>
/// 主视图模型，作为应用程序的核心协调者
/// </summary>
public partial class MainViewModel : ObservableObject
{
    // 组合子 ViewModel
    [ObservableProperty]
    private RequestTreeViewModel _treeViewModel;
    
    [ObservableProperty]
    private RequestDetailViewModel _detailViewModel;
    
    // 通过事件订阅实现组件间通信
    public MainViewModel(...)
    {
        TreeViewModel.NodeSelected += OnNodeSelected;
    }
}
```

### Model 组织

**分层设计**：

- **领域模型** (`Infrastructure/Domain/`): 用于持久化和业务逻辑
- **视图模型** (`Models/`): 用于 UI 绑定，包含 ObservableCollection

```csharp
// 领域模型 - 用于持久化
public class RequestTreeItem
{
    public string Id { get; set; }
    public TreeNodeTypeEnum NodeType { get; set; }
    public List<RequestTreeItem> Children { get; set; }
}

// 视图模型 - 用于 UI 绑定
public partial class RequestFolder : RequestNode
{
    [ObservableProperty]
    private ObservableCollection<RequestNode> _children = new();
}
```

### View 组织

- **XAML**: 纯 UI 布局，使用数据绑定
- **Code-behind**: 仅处理 UI 事件转发

```csharp
// MainWindow.xaml.cs - 仅转发事件
private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
{
    if (DataContext is MainViewModel viewModel && e.NewValue is RequestNode node)
    {
        viewModel.TreeViewModel.OnNodeSelected(node);
    }
}
```

## 文档标准

### XML 文档注释

**完整格式**（推荐用于公共 API）：

```csharp
/// <summary>
/// 请求树视图模型，管理左侧树形结构的显示和操作
/// </summary>
/// <remarks>
/// <para>该类负责管理请求集合的树形结构，包括：</para>
/// <list type="bullet">
///   <item><description>文件夹和请求节点的增删操作</description></item>
///   <item><description>树节点选择的处理和事件通知</description></item>
/// </list>
/// </remarks>
public partial class RequestTreeViewModel : ObservableObject
```

**属性注释**：

```csharp
/// <summary>
/// 节点唯一标识符
/// </summary>
/// <remarks>
/// 使用 GUID 自动生成，用于持久化和节点查找
/// </remarks>
[ObservableProperty]
private string _id = Guid.NewGuid().ToString();
```

**方法注释**：

```csharp
/// <summary>
/// 添加新文件夹节点
/// </summary>
/// <returns>表示异步操作的任务</returns>
/// <remarks>
/// <para>添加位置规则：</para>
/// <list type="bullet">
///   <item><description>未选中节点：添加到根级别</description></item>
///   <item><description>选中文件夹：添加为该文件夹的子节点</description></item>
/// </list>
/// </remarks>
[RelayCommand]
private async Task AddFolder() { }
```

### 简洁格式

```csharp
/// <summary>
/// 请求项
/// </summary>
public class RequestItem
{
    /// <summary>
    /// 请求动词
    /// </summary>
    public RequestVerbEnum RequestVerb { set; get; }
}
```

## 现代 C# 特性

### 主构造函数（C# 12）

```csharp
public class BusinessException(string message) : Exception(message);

public class TextBoxWriter(TextBox textBox) : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}
```

### 集合初始化器

```csharp
public List<RequestTreeItem> Children { get; set; } = [];
public Dictionary<string, string> Header { set; get; } = [];
```

### Switch 表达式

```csharp
return verb switch
{
    RequestVerbEnum.Get => new SolidColorBrush(Color.FromRgb(97, 175, 239)),
    RequestVerbEnum.Post => new SolidColorBrush(Color.FromRgb(73, 204, 144)),
    _ => new SolidColorBrush(Colors.Gray)
};
```

### 模式匹配

```csharp
if (SelectedNode is RequestFolder folder)
{
    folder.Children.Add(newFolder);
}
```

## 依赖注入

### 服务注册

```csharp
// App.xaml.cs
AppHost = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // 配置选项
        services.Configure<AppOption>(context.Configuration);
        
        // 单例服务
        services.AddSingleton<IRepository<GlobalSettings>, JsonRepository<GlobalSettings>>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        
        // 瞬态服务
        services.AddTransient<IJsonTransform, SnowIdTransfrom>();
        services.AddTransient<JsonTransformPipeline>();
    })
    .Build();
```

### 服务生命周期

| 生命周期      | 使用场景               | 示例                                        |
| :-------- | :----------------- | :---------------------------------------- |
| Singleton | 全局状态、ViewModel、主窗口 | `MainViewModel`, `IRepository<T>`         |
| Transient | 轻量级、无状态服务          | `IJsonTransform`, `JsonTransformPipeline` |

## 异常处理

### 自定义异常

```csharp
// 业务异常
public class BusinessException(string message) : Exception(message);
```

### 全局异常处理

```csharp
// App.xaml.cs - 注册异常处理器
this.DispatcherUnhandledException += OnDispatcherUnhandledException;
AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

// GlobalExceptionHandler.cs - 统一处理
public static void Handle(Exception ex)
{
    if (ex is BusinessException businessEx)
    {
        ShowBusinessError(businessEx);  // 友好提示
    }
    else
    {
        Log(ex);
        ShowSystemError();  // 通用错误提示
    }
}
```

## 配置管理

### 配置文件

```json
// appsettings.json
{
  "BaseUrl": "localhost:",
  "ConnectionString": "Data Source=test.db",
  "DbType": "Sqlite"
}
```

### 配置类

```csharp
public class AppOption
{
    public string? BaseUrl { get; set; }
    public string? ConnectionString { get; set; }
    public string? DbType { get; set; }
    public bool IsAutoCloseConnection { get; set; } = true;
}
```

### 使用方式

```csharp
public class DatabaseService : IDatabaseService
{
    public DatabaseService(IOptions<AppOption> options)
    {
        var appOption = options.Value;
        // ...
    }
}
```

## 枚举定义

```csharp
public enum RequestVerbEnum
{
    /// <summary>
    /// GET 请求
    /// </summary>
    [Description("GET")]
    Get = 1,

    /// <summary>
    /// POST 请求
    /// </summary>
    [Description("POST")]
    Post = 2,
}
```

**配合扩展方法使用**：

```csharp
public static string GetDescription(this Enum value)
{
    var field = value.GetType().GetField(value.ToString());
    var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
    return attribute?.Description ?? value.ToString();
}
```

## 对象映射

使用 **Mapster** 进行对象映射：

```csharp
public static void Configure()
{
    TypeAdapterConfig<RequestTreeItem, RequestFolder>
        .NewConfig()
        .AfterMapping((src, dest) =>
        {
            dest.NodeType = TreeNodeTypeEnum.Folder;
        });
}

// 使用
var node = item.Adapt<RequestFolder>();
```

## 设计模式

### 责任链模式

```csharp
public class JsonTransformPipeline(IEnumerable<IJsonTransform> transforms)
{
    public string Execute(string json)
    {
        return transforms.Aggregate(json, (current, transform) => transform.Transform(current));
    }
}
```

### 仓储模式

```csharp
public interface IRepository<T> where T : class, new()
{
    Task<T> GetAsync();
    Task SaveAsync(T entity);
    Task ResetAsync();
}
```

### 观察者模式

```csharp
public event Action<RequestNode>? NodeSelected;

// 订阅
TreeViewModel.NodeSelected += OnNodeSelected;

// 触发
NodeSelected?.Invoke(node);
```

## 文件组织

### 目录结构

| 目录            | 内容          | 命名空间                                     |
| :------------ | :---------- | :--------------------------------------- |
| `ViewModels/` | ViewModel 类 | `ApiTestFramework.ViewModels`            |
| `Models/`     | 视图专用模型      | `ApiTestFramework.Models`                |
| `Converters/` | 值转换器        | `ApiTestFramework.Converters`            |
| `Components/` | UI 组件       | `ApiTestFramework.Components`            |
| `Domain/`     | 领域模型        | `ApiTestFramework.Infrastructure.Domain` |
| `Enum/`       | 枚举定义        | `ApiTestFramework.Infrastructure.Enum`   |
| `Interface/`  | 服务接口        | `ApiTestFramework.Service.Interface`     |
| `Services/`   | 服务实现        | `ApiTestFramework.Service.Services`      |

### 文件命名

- 一个文件一个主要类型
- 文件名与类名一致
- 相关类型可放在同一文件（如 `RequestFolder.cs` 包含 `RequestFolder` 和 `RequestItemNode`）

## 代码风格总结

1. **清晰的分层架构**：主项目、服务层、基础设施层职责分明
2. **现代 MVVM 实践**：使用 CommunityToolkit.Mvvm 源生成器，减少样板代码
3. **完善的依赖注入**：使用 Microsoft.Extensions.DependencyInjection
4. **详细的 XML 注释**：公共 API 都有完整的文档注释
5. **一致的命名约定**：类、方法、属性命名规范统一
6. **现代 C# 语法**：使用主构造函数、switch 表达式、模式匹配等新特性

