using ApiTestFramework.Components;
using ApiTestFramework.Infrastructure.Helper;
using ApiTestFramework.Infrastructure.JsonTransform;
using ApiTestFramework.Service.Interface;
using System.IO;
using System.Windows;

namespace ApiTestFramework;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    public MainWindow( )
    {
        InitializeComponent();
        Console.SetOut(new TextBoxWriter(LogTextBox));
    }

    private async void GenerateSeedData_Click(object sender, RoutedEventArgs e)
    {
    }

}
