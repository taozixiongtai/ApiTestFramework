using System.Windows;
using ApiTestFramework.Helper;
using System.Collections.Generic;
using System;
using System.IO;

namespace ApiTestFramework;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void GenerateSeedData_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var connectionString = "Server=192.168.100.200;Port=3306;Database=mbse_platform;Uid=root;Pwd=P@88@123;SslMode=None;AllowPublicKeyRetrieval=true;CharSet=utf8mb4;";
            var db = new DatabaseService(connectionString);

            var jsonService = new JsonService();

            var seedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Seed", "table", "table.json");
            if (!File.Exists(seedPath))
            {
                seedPath = Path.Combine("Seed", "table", "table.json");
            }

            var parsed = jsonService.ParseDirectory(seedPath);

            foreach (var kv in parsed)
            {
                db.InsertData(kv.Key, kv.Value);
            }

            MessageBox.Show("种子数据已生成", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"生成失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}