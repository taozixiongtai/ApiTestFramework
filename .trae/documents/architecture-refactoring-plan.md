# 项目架构重构可行性评估与实施计划

## 一、可行性评估

### 1. 当前架构分析

```
当前结构:
ApiTestFramework (UI层)
├── 依赖 → ApiTestFramework.Service
├── ViewModels, Components, Converters, Models, Helper, Infrastructure, Mapper, Seed

ApiTestFramework.Service (服务层)
├── 依赖 → ApiTestFramework.Infrastructure
├── Interface, Services

ApiTestFramework.Infrastructure (基础设施层)
├── APP, Domain, Enum, Exceptions, Extensions, Helper, JsonTransform
```

### 2. 目标架构分析

```
目标结构:
ApiTestFramework.UI (表现层)
├── Views, ViewModels, Controls, Converters

ApiTestFramework.Application (应用层)
├── Interfaces, Services, DTOs, Mappers

ApiTestFramework.Domain (领域层)
├── Entities, Enums, ValueObjects

ApiTestFramework.Infrastructure (基础设施层)
├── Persistence, Json, FileSystem, Seed, Exceptions
```

### 3. 可行性结论：✅ 可行

**理由：**
1. 当前项目已有分层基础，重构风险可控
2. 文件数量适中（约50个cs文件），工作量可接受
3. 目标架构符合Clean Architecture/DDD最佳实践
4. 依赖关系可以平滑迁移

**注意事项：**
1. 需要更新所有命名空间（约50个文件）
2. 需要重新配置项目引用关系
3. 需要更新.slnx解决方案文件
4. 部分代码可能需要适配新架构

---

## 二、文件映射关系

### ApiTestFramework.UI (新建，重命名原ApiTestFramework)

| 原路径 | 新路径 |
|--------|--------|
| ApiTestFramework/ViewModels/* | ApiTestFramework.UI/ViewModels/* |
| ApiTestFramework/Components/* | ApiTestFramework.UI/Controls/* |
| ApiTestFramework/Converters/* | ApiTestFramework.UI/Converters/* |
| ApiTestFramework/MainWindow.xaml | ApiTestFramework.UI/Views/MainWindow.xaml |
| ApiTestFramework/SettingsWindow.xaml | ApiTestFramework.UI/Views/SettingsWindow.xaml |
| ApiTestFramework/FilePreviewWindow.xaml | ApiTestFramework.UI/Views/FilePreviewWindow.xaml |
| ApiTestFramework/App.xaml | ApiTestFramework.UI/App.xaml |

### ApiTestFramework.Application (新建，重命名原ApiTestFramework.Service)

| 原路径 | 新路径 |
|--------|--------|
| ApiTestFramework.Service/Interface/* | ApiTestFramework.Application/Interfaces/* |
| ApiTestFramework.Service/Services/* | ApiTestFramework.Application/Services/* |
| ApiTestFramework/Mapper/DataMapper.cs | ApiTestFramework.Application/Mappers/DataMapper.cs |
| ApiTestFramework/Models/* | ApiTestFramework.Application/DTOs/* |

### ApiTestFramework.Domain (新建，从Infrastructure拆分)

| 原路径 | 新路径 |
|--------|--------|
| ApiTestFramework.Infrastructure/Domain/* | ApiTestFramework.Domain/Entities/* |
| ApiTestFramework.Infrastructure/Enum/* | ApiTestFramework.Domain/Enums/* |

### ApiTestFramework.Infrastructure (重构)

| 原路径 | 新路径 |
|--------|--------|
| ApiTestFramework.Infrastructure/APP/* | ApiTestFramework.Infrastructure/Configuration/* |
| ApiTestFramework.Infrastructure/Helper/FileHelper.cs | ApiTestFramework.Infrastructure/FileSystem/* |
| ApiTestFramework.Infrastructure/Helper/JsonHelper.cs | ApiTestFramework.Infrastructure/Json/* |
| ApiTestFramework.Infrastructure/JsonTransform/* | ApiTestFramework.Infrastructure/Json/* |
| ApiTestFramework.Infrastructure/Exceptions/* | ApiTestFramework.Infrastructure/Exceptions/* |
| ApiTestFramework/Seed/* | ApiTestFramework.Infrastructure/Seed/* |
| ApiTestFramework.Infrastructure/Extensions/* | ApiTestFramework.Infrastructure/Extensions/* |
| ApiTestFramework.Infrastructure/Helper/SnowflakeIdHelper.cs | ApiTestFramework.Infrastructure/IdGenerator/* |

---

## 三、依赖关系调整

### 当前依赖
```
ApiTestFramework → ApiTestFramework.Service → ApiTestFramework.Infrastructure
```

### 目标依赖
```
ApiTestFramework.UI → ApiTestFramework.Application → ApiTestFramework.Domain
                              ↓
                   ApiTestFramework.Infrastructure
```

### 关键原则
- Domain层：无任何外部依赖，纯C#类库
- Application层：依赖Domain，定义接口
- Infrastructure层：依赖Domain，实现Application定义的接口
- UI层：依赖Application和Infrastructure（用于DI注册）

---

## 四、实施步骤

### 阶段一：创建新项目结构

1. 创建 ApiTestFramework.Domain 项目
   - 创建 Entities, Enums, ValueObjects 文件夹
   - 配置 .csproj (net10.0, 无外部依赖)

2. 创建 ApiTestFramework.Application 项目
   - 创建 Interfaces, Services, DTOs, Mappers 文件夹
   - 配置 .csproj，添加 Domain 引用

3. 重命名 ApiTestFramework.Infrastructure
   - 创建新文件夹结构
   - 移动文件到对应位置

4. 重命名 ApiTestFramework 为 ApiTestFramework.UI
   - 创建 Views, Controls 文件夹
   - 移动文件到对应位置

### 阶段二：迁移Domain层

1. 迁移实体类
   - RequestItem.cs → Entities/
   - RequestTreeItem.cs → Entities/
   - SeedDataItem.cs → Entities/
   - GlobalSettings.cs → Entities/
   - DynamicJsonObject.cs → Entities/ 或 ValueObjects/

2. 迁移枚举
   - RequestVerbEnum.cs → Enums/
   - TreeNodeTypeEnum.cs → Enums/
   - TreeNodeMenuActionEnum.cs → Enums/

3. 更新命名空间
   - ApiTestFramework.Infrastructure.Domain → ApiTestFramework.Domain.Entities
   - ApiTestFramework.Infrastructure.Enum → ApiTestFramework.Domain.Enums

### 阶段三：迁移Application层

1. 迁移接口
   - IDatabaseService.cs → Interfaces/
   - IHttpClientService.cs → Interfaces/
   - IRepository.cs → Interfaces/
   - ISeedDataService.cs → Interfaces/
   - ITestHandlerService.cs → Interfaces/

2. 迁移服务
   - DatabaseService.cs → Services/
   - HttpClientService.cs → Services/
   - JsonRepository.cs → Services/
   - SeedDataService.cs → Services/
   - TestHandlerService.cs → Services/

3. 迁移DTOs和Mappers
   - Models/* → DTOs/
   - Mapper/DataMapper.cs → Mappers/

4. 更新命名空间
   - ApiTestFramework.Service.Interface → ApiTestFramework.Application.Interfaces
   - ApiTestFramework.Service.Services → ApiTestFramework.Application.Services

### 阶段四：重构Infrastructure层

1. 创建新文件夹结构
   - Persistence/, Json/, FileSystem/, Seed/, Exceptions/, Configuration/, Extensions/, IdGenerator/

2. 迁移文件
   - APP/* → Configuration/
   - Helper/FileHelper.cs → FileSystem/
   - Helper/JsonHelper.cs, JsonTransform/* → Json/
   - Exceptions/* → Exceptions/
   - Extensions/* → Extensions/
   - Helper/SnowflakeIdHelper.cs → IdGenerator/

3. 更新命名空间

### 阶段五：重构UI层

1. 创建文件夹结构
   - Views/, Controls/, ViewModels/, Converters/

2. 迁移文件
   - MainWindow.xaml → Views/
   - SettingsWindow.xaml → Views/
   - FilePreviewWindow.xaml → Views/
   - Components/* → Controls/
   - ViewModels/* → ViewModels/
   - Converters/* → Converters/

3. 删除原 Infrastructure, Mapper, Models, Helper, Seed 文件夹

4. 更新命名空间
   - ApiTestFramework → ApiTestFramework.UI

### 阶段六：更新配置和依赖

1. 更新解决方案文件 ApiTestFramework.slnx
2. 更新各项目的项目引用
3. 更新 NuGet 包引用
4. 更新 appsettings.json 配置

### 阶段七：编译和测试

1. 执行 dotnet build 检查编译错误
2. 修复命名空间引用问题
3. 运行应用程序验证功能

---

## 五、风险与应对

| 风险 | 影响 | 应对措施 |
|------|------|----------|
| 命名空间更新遗漏 | 编译错误 | 使用全局搜索替换 |
| 依赖注入配置失效 | 运行时错误 | 检查App.xaml.cs中的DI配置 |
| 文件路径硬编码 | 功能异常 | 检查Seed等文件路径引用 |
| XAML引用失效 | UI显示异常 | 检查XAML中的命名空间引用 |

---

## 六、工作量估算

| 任务 | 文件数量 | 预估复杂度 |
|------|----------|------------|
| 创建新项目结构 | 4个项目 | 低 |
| 迁移Domain层 | ~10个文件 | 中 |
| 迁移Application层 | ~15个文件 | 中 |
| 重构Infrastructure层 | ~10个文件 | 中 |
| 重构UI层 | ~20个文件 | 高 |
| 更新配置和依赖 | 配置文件 | 低 |
| 编译修复和测试 | 全部 | 中 |

**总体评估：中等复杂度，建议分阶段实施，每阶段完成后验证编译通过。**
