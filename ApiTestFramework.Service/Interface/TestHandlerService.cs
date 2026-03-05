using ApiTestFramework.Infrastructure.Helper;
using ApiTestFramework.Infrastructure.JsonTransform;
using ApiTestFramework.Service.Services;

namespace ApiTestFramework.Service.Interface
{
    public class TestHandlerService(IDatabaseService databaseService, JsonTransformPipeline jsonTransformPipeline, IHttpClientService httpClientService) : ITestHandlerService
    {
        public async Task ExecuteTestCase()
        {
            // 1. 登录
            // 2. 生成种子
            // 3. 执行种子替换
            // 4. 插入数据
            // 5. 调用接口
            // 6. 删除种子数据

            var path = Path.Combine(AppContext.BaseDirectory, "Seed", "table");
            var allFile = await FileHelper.ReadAllJsonFiles(path);
            foreach (var file in allFile)
            {
                // 执行替换字符串
                var DynamicJsonObject = JsonHelper.ParseDirectory(jsonTransformPipeline.Execute(file.Value));
                foreach (var item in DynamicJsonObject)
                {
                    databaseService.InsertData(item.Key, item.Value);
                }
            }
        }
    }
}
