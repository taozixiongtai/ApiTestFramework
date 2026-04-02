using ApiTestFramework.Components;
using ApiTestFramework.Infrastructure.APP;
using ApiTestFramework.Infrastructure.Domain;
using ApiTestFramework.Infrastructure.JsonTransform;
using ApiTestFramework.Mapper;
using ApiTestFramework.Service.Interface;
using ApiTestFramework.Service.Services;
using ApiTestFramework.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace ApiTestFramework;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        DataMapper.Configure();

        AppHost = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<AppOption>(context.Configuration);

                services.AddSingleton<IRepository<GlobalSettings>, JsonRepository<GlobalSettings>>();
                services.AddSingleton<IRepository<List<RequestTreeItem>>, JsonRepository<List<RequestTreeItem>>>();

                services.AddSingleton<IHttpClientService, HttpClientService>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<IDatabaseService, DatabaseService>();

                services.AddTransient<IJsonTransform, SnowIdTransfrom>();
                services.AddTransient<JsonTransformPipeline>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        base.OnStartup(e);

        this.DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = AppHost.Services.GetRequiredService<MainViewModel>();
        mainWindow.Show();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        GlobalExceptionHandler.Handle(e.Exception);
        e.Handled = true;
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
