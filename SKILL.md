---
name: "code-conventions"
description: "定义 ApiTestFramework 项目的编码规范和约定。在编写、修改或审查代码时调用以确保一致性。"
---

# 代码规范

## 通用规则

1. **语言**: 代码注释和文档使用中文，除非另有说明
2. **命名约定**:
   - 类、方法、属性: PascalCase
   - 局部变量、参数: camelCase
   - 私有字段: _camelCase (带下划线前缀)
   - 常量:  UPPER_CASE
   - 接口: IPascalCase (带 I 前缀)
   - 命名空间：用文件范围限定格式

## 项目结构

```
ApiTestFramework/
├── ApiTestFramework/           # 主 WPF 应用程序
│   ├── Converters/             # WPF 值转换器
│   ├── Mapper/                 # 对象映射配置
│   ├── Models/                 # UI 绑定模型 (MVVM)
│   └── ViewModels/             # MVVM 视图模型
├── ApiTestFramework.Infrastructure/  # 基础设施层
│   ├── APP/                    # 应用程序配置
│   ├── Domain/                 # 领域实体
│   ├── Enum/                   # 枚举
│   ├── Exceptions/             # 自定义异常
│   ├── Helper/                 # 工具帮助类
│   └── Service/                # 基础设施服务
└── ApiTestFramework.Service/   # 业务服务,所有service都放在这里
```

## 代码风格

### 注释
- 公共 API 使用 XML 文档注释
- 注释应简洁明了
- 示例:
  ```csharp
  /// <summary>
  /// 保存数据到文件
  /// </summary>
  public async Task SaveAsync()
  ```

### MVVM 模式
- 使用 CommunityToolkit.Mvvm 特性:
  - `[ObservableProperty]` 用于属性
  - `[RelayCommand]` 用于命令
- 示例:
  ```csharp
  public partial class MyViewModel : ObservableObject
  {
      [ObservableProperty]
      private string _name;

      [RelayCommand]
      private void DoSomething() { }
  }
  ```

### 依赖注入
- 在 `App.xaml.cs` 中注册服务
- 使用构造函数注入
- 服务应放在 `Infrastructure/Service` 或 `Service` 项目中

### 数据映射
- 使用 Mapster 进行对象映射
- 在 `Mapper/DataMapper.cs` 中配置映射
- 应用程序启动时调用 `DataMapper.Configure()`

### 文件组织
- 每个文件一个类
- 文件名与类名匹配
- 使用文件夹按功能/层组织

## 分层职责

| 层 | 职责 |
|---|---|
| Models | UI 绑定数据模型，带 MVVM 特性 |
| ViewModels | UI 逻辑和状态管理 |
| Infrastructure/Domain | 纯数据模型，用于持久化 |
| Infrastructure/Service | 数据访问和基础设施服务 |
| Service | 业务逻辑服务 |

## 最佳实践

1. **关注点分离**: 保持 UI、业务逻辑和数据访问分离
2. **异步编程**: I/O 操作使用 async 方法
3. **错误处理**: 业务错误使用 `BusinessException`
4. **空值安全**: 启用可空引用类型，适当处理空值
