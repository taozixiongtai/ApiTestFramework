using ApiTestFramework.Infrastructure.APP;
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
            await Login();
            // 2. 生成种子
            // 3. 执行种子替换
            // 4. 插入数据
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

            // 5. 调用接口
            todo 定义接口的class
            // 6. 删除种子数据
        }


        private async Task Login()
        {
            if (!string.IsNullOrEmpty(APPGloal.Token))
            {
                return;
            }
            var loginUrl = "http://example.com/api/login";
            var loginData = new { username = "user", password = "pass" };
            var response = await httpClientService.PostStringAsync(loginUrl, loginData);
            var token = JsonHelper.GetValue(response, "Token");
            APPGloal.Token = token ?? string.Empty;
        }
    }
}
