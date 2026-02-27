using ApiTestFramework.Components;
using ApiTestFramework.Infrastructure.Helper;
using ApiTestFramework.Service.Interface;
using System.IO;
using System.Windows;

namespace ApiTestFramework;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IDatabaseService _databaseService;

    public MainWindow(IDatabaseService databaseService)
    {
        InitializeComponent();
        Console.SetOut(new TextBoxWriter(LogTextBox));
        _databaseService = databaseService;
    }

    private async void GenerateSeedData_Click(object sender, RoutedEventArgs e)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Seed", "table");
        var allFile = FileHelper.ReadAllJsonFiles(path);
        foreach (var file in allFile)
        {
            var DynamicJsonObject = JsonHelper.ParseDirectory(file.Value);
            foreach (var item in DynamicJsonObject)
            {
                _databaseService.InsertData(item.Key, item.Value);
            }

        }
    }

}
