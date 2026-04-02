# 种子数据生成器用户控件实现计划

## 需求概述

在 ApiTestFramework 项目中新增一个种子数据生成器用户控件，用于管理和执行种子数据。

### 功能需求

1. **UI 界面**：
   - 在左侧工具栏"新建请求"按钮旁边添加"新增种子数据"按钮
   - 右侧显示种子数据管理界面，包含：
     - 上传文件按钮（选择 JSON 文件）
     - 已上传文件列表
     - 保存按钮
     - 执行按钮

2. **核心功能**：
   - 上传 JSON 文件并保存到 `Seed` 文件夹
   - 显示已上传的文件列表
   - 执行种子数据插入到数据库（调用现有的 `DatabaseService.InsertData`）

---

## 实现步骤

### 步骤 1：创建服务接口和实现

**文件**: `ApiTestFramework.Service/Interface/ISeedDataService.cs`

```csharp
public interface ISeedDataService
{
    Task<List<string>> GetSeedFilesAsync();
    Task SaveSeedFileAsync(string fileName, string content);
    Task ExecuteSeedDataAsync();
    Task DeleteSeedFileAsync(string fileName);
}
```

**文件**: `ApiTestFramework.Service/Services/SeedDataService.cs`

实现：
- `GetSeedFilesAsync()`: 获取 Seed 文件夹下所有 JSON 文件名
- `SaveSeedFileAsync()`: 保存 JSON 内容到 Seed 文件夹
- `ExecuteSeedDataAsync()`: 执行种子数据插入（参考 `TestHandlerService.ExecuteTestCase` 的逻辑）
- `DeleteSeedFileAsync()`: 删除指定的种子文件

### 步骤 2：创建 ViewModel

**文件**: `ApiTestFramework/ViewModels/SeedDataViewModel.cs`

属性：
- `ObservableCollection<string> SeedFiles` - 种子文件列表
- `string? SelectedFile` - 当前选中的文件
- `string FileContent` - 当前编辑的内容
- `bool IsExecuting` - 是否正在执行

命令：
- `UploadFileCommand` - 上传文件
- `SaveCommand` - 保存文件
- `ExecuteCommand` - 执行种子数据
- `DeleteFileCommand` - 删除文件
- `RefreshCommand` - 刷新文件列表

### 步骤 3：创建用户控件

**文件**: `ApiTestFramework/Components/SeedDataControl.xaml`

UI 布局：
- 顶部工具栏：上传按钮、保存按钮、执行按钮
- 左侧：文件列表（ListBox）
- 右侧：文件内容编辑区（TextBox，支持多行编辑）

**文件**: `ApiTestFramework/Components/SeedDataControl.xaml.cs`

Code-behind 仅处理 UI 事件转发

### 步骤 4：修改 MainWindow.xaml

在左侧工具栏添加"新增种子数据"按钮：

```xml
<Button Content="新增种子数据" 
        Command="{Binding ShowSeedDataCommand}"
        Background="Transparent"
        BorderThickness="1"
        BorderBrush="#CCC"
        Padding="8,4"
        Margin="0,0,5,0"/>
```

### 步骤 5：修改 MainViewModel

添加：
- `ShowSeedDataCommand` 命令
- `SeedDataViewModel` 属性
- 在构造函数中注入 `ISeedDataService`
- 切换到种子数据控件的逻辑

### 步骤 6：注册依赖注入

**文件**: `ApiTestFramework/App.xaml.cs`

```csharp
services.AddSingleton<ISeedDataService, SeedDataService>();
services.AddSingleton<SeedDataViewModel>();
```

### 步骤 7：配置 Seed 文件夹复制

**文件**: `ApiTestFramework/ApiTestFramework.csproj`

确保 Seed 文件夹在编译时复制到输出目录：

```xml
<ItemGroup>
  <None Update="Seed\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

---

## 文件清单

### 新增文件

| 文件路径 | 说明 |
|---------|------|
| `ApiTestFramework.Service/Interface/ISeedDataService.cs` | 种子数据服务接口 |
| `ApiTestFramework.Service/Services/SeedDataService.cs` | 种子数据服务实现 |
| `ApiTestFramework/ViewModels/SeedDataViewModel.cs` | 种子数据视图模型 |
| `ApiTestFramework/Components/SeedDataControl.xaml` | 种子数据用户控件 XAML |
| `ApiTestFramework/Components/SeedDataControl.xaml.cs` | 种子数据用户控件代码 |

### 修改文件

| 文件路径 | 修改内容 |
|---------|---------|
| `ApiTestFramework/MainWindow.xaml` | 添加"新增种子数据"按钮 |
| `ApiTestFramework/ViewModels/MainViewModel.cs` | 添加种子数据相关逻辑 |
| `ApiTestFramework/App.xaml.cs` | 注册新服务和 ViewModel |
| `ApiTestFramework/ApiTestFramework.csproj` | 配置 Seed 文件夹复制 |

---

## 关键技术点

### Seed 文件夹路径处理

```csharp
// 获取 Seed 文件夹路径
var seedPath = Path.Combine(AppContext.BaseDirectory, "Seed");

// 确保目录存在
if (!Directory.Exists(seedPath))
{
    Directory.CreateDirectory(seedPath);
}
```

### 文件上传对话框

```csharp
var openFileDialog = new Microsoft.Win32.OpenFileDialog
{
    Filter = "JSON 文件 (*.json)|*.json",
    Multiselect = true
};

if (openFileDialog.ShowDialog() == true)
{
    foreach (var fileName in openFileDialog.FileNames)
    {
        var content = await File.ReadAllTextAsync(fileName);
        var destFileName = Path.GetFileName(fileName);
        await _seedDataService.SaveSeedFileAsync(destFileName, content);
    }
}
```

### 执行种子数据

参考 `TestHandlerService.ExecuteTestCase` 的实现：
1. 读取 Seed 文件夹下所有 JSON 文件
2. 使用 `JsonTransformPipeline` 执行变量替换
3. 使用 `JsonHelper.ParseDirectory` 解析 JSON
4. 调用 `IDatabaseService.InsertData` 插入数据

---

## 遵循项目规范

1. **命名约定**：
   - ViewModel: `SeedDataViewModel`
   - Service 接口: `ISeedDataService`
   - Service 实现: `SeedDataService`
   - UserControl: `SeedDataControl`

2. **MVVM 模式**：
   - 使用 `[ObservableProperty]` 和 `[RelayCommand]` 特性
   - 私有字段使用下划线前缀

3. **XML 文档注释**：
   - 公共 API 都有完整的文档注释

4. **依赖注入**：
   - 服务使用 Singleton 生命周期
