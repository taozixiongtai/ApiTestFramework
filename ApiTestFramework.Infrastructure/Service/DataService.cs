using ApiTestFramework.Infrastructure.Domain;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiTestFramework.Infrastructure.Service
{
    /// <summary>
    /// 数据持久化服务
    /// </summary>
    public class DataService
    {
        private readonly string _dataFilePath;
        private AppData _appData;
        private readonly JsonSerializerOptions _jsonOptions;

        public DataService(string? dataFilePath = null)
        {
            _dataFilePath = dataFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appdata.json");
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _appData = new AppData();
        }

        /// <summary>
        /// 获取应用数据
        /// </summary>
        public AppData AppData => _appData;

        /// <summary>
        /// 初始化数据，从文件加载或创建默认数据
        /// </summary>
        public async Task InitializeAsync()
        {
            if (File.Exists(_dataFilePath))
            {
                await LoadAsync();
            }
            else
            {
                _appData = new AppData();
                await SaveAsync();
            }
        }

        /// <summary>
        /// 从文件加载数据
        /// </summary>
        public async Task LoadAsync()
        {
            if (!File.Exists(_dataFilePath))
            {
                _appData = new AppData();
                return;
            }

            var json = await File.ReadAllTextAsync(_dataFilePath);
            var data = JsonSerializer.Deserialize<AppData>(json, _jsonOptions);
            _appData = data ?? new AppData();
        }

        /// <summary>
        /// 保存数据到文件
        /// </summary>
        public async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(_appData, _jsonOptions);
            await File.WriteAllTextAsync(_dataFilePath, json);
        }

        /// <summary>
        /// 获取请求树
        /// </summary>
        public List<RequestTreeItem> GetRequestTree() => _appData.RequestTree;

        /// <summary>
        /// 保存请求树
        /// </summary>
        public async Task SaveRequestTreeAsync(List<RequestTreeItem> tree)
        {
            _appData.RequestTree = tree;
            await SaveAsync();
        }

        /// <summary>
        /// 获取全局设置
        /// </summary>
        public GlobalSettings GetSettings() => _appData.Settings;

        /// <summary>
        /// 保存全局设置
        /// </summary>
        public async Task SaveSettingsAsync(GlobalSettings settings)
        {
            _appData.Settings = settings;
            await SaveAsync();
        }

        /// <summary>
        /// 更新Token
        /// </summary>
        public async Task UpdateTokenAsync(string token)
        {
            _appData.Settings.Token = token;
            await SaveAsync();
        }

        /// <summary>
        /// 更新变量
        /// </summary>
        public async Task UpdateVariablesAsync(Dictionary<string, string> variables)
        {
            _appData.Settings.Variables = variables;
            await SaveAsync();
        }

        /// <summary>
        /// 更新全局请求头
        /// </summary>
        public async Task UpdateGlobalHeadersAsync(Dictionary<string, string> headers)
        {
            _appData.Settings.GlobalHeaders = headers;
            await SaveAsync();
        }

        /// <summary>
        /// 更新基础URL
        /// </summary>
        public async Task UpdateBaseUrlAsync(string baseUrl)
        {
            _appData.Settings.BaseUrl = baseUrl;
            await SaveAsync();
        }
    }
}
