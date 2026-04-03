using ApiTestFramework.Infrastructure.Configuration;
using ApiTestFramework.Infrastructure.FileSystem;
using ApiTestFramework.Infrastructure.Json;
using ApiTestFramework.Application.Interfaces;

namespace ApiTestFramework.Application.Services;

public class TestHandlerService(IDatabaseService databaseService, JsonTransformPipeline jsonTransformPipeline, IHttpClientService httpClientService) : ITestHandlerService
{
    public async Task ExecuteTestCase()
    {

        await Login();
        var path = Path.Combine(AppContext.BaseDirectory, "Seed", "table");
        var allFile = await FileHelper.ReadAllJsonFiles(path);
        foreach (var file in allFile)
        {
            var DynamicJsonObject = JsonHelper.ParseDirectory(jsonTransformPipeline.Execute(file.Value));
            foreach (var item in DynamicJsonObject)
            {
                databaseService.InsertData(item.Key, item.Value);
            }
        }

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
