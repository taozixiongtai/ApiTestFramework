namespace ApiTestFramework.Infrastructure.APP
{
    /// <summary>
    /// 保存应用配置
    /// </summary>
    public class AppOption
    {
        public string? BaseUrl { get; set; }

        public string? LoginUrl { get; set; }

        public string? LoginPassword { get; set; }

        public string? LoginUserName { get; set; }

        public Dictionary<string, string>? RequestHeader { set; get; }
    }
}
