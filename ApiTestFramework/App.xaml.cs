using ApiTestFramework.Components;
using ApiTestFramework.Infrastructure.APP;
using ApiTestFramework.Infrastructure.JsonTransform;
using ApiTestFramework.Service.Interface;
using ApiTestFramework.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace ApiTestFramework;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // 绑定配置
                services.Configure<AppOption>(context.Configuration);
                // 注册服务
                // todo 把所有的window都注册进来,应该要写一个方法 就像addcontrollers那样
                services.AddSingleton<MainWindow>();
                services.AddSingleton<IDatabaseService, DatabaseService>();

                // todo 写个方法批量注册责任链的接口
                services.AddTransient<IJsonTransform, SnowIdTransfrom>();
                services.AddTransient<JsonTransformPipeline>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        base.OnStartup(e);
        // UI线程异常
        this.DispatcherUnhandledException += OnDispatcherUnhandledException;

        // 非UI线程异常
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        // Task异常
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        GlobalExceptionHandler.Handle(e.Exception);

        e.Handled = true; // 防止程序崩溃
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        GlobalExceptionHandler.Handle(ex);
    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        GlobalExceptionHandler.Handle(e.Exception);
        e.SetObserved();
    }
}

