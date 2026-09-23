namespace ApiTestFramework.Infrastructure.Configuration
{
    public class AppOption
    {
        public string? BaseUrl { get; set; }

        public string? LoginUrl { get; set; }

        public string? LoginPassword { get; set; }

        public string? LoginUserName { get; set; }

        public Dictionary<string, string>? RequestHeader { set; get; }
    }
}
