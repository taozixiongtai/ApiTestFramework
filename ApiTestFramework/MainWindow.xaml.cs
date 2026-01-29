using System.Windows;
using ApiTestFramework.Helper;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Windows.Threading;
using RestSharp;

namespace ApiTestFramework;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TextWriter _originalOut;
    private TextWriter _originalError;

    public MainWindow()
    {
        InitializeComponent();

        // Redirect Console output to the LogTextBox
        _originalOut = Console.Out;
        _originalError = Console.Error;

        var writer = new TextBoxWriter(LogTextBox);
        Console.SetOut(writer);
        Console.SetError(writer);
    }

    private async Task GenerateSeedData_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SnowflakeIdGenerator snowflakeIdGenerator = new SnowflakeIdGenerator();
            APP.Dic.Add("projectId", "");
            APP.Dic.Add("tableId", snowflakeIdGenerator.NextId().ToString());

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


            var client = new HttpApiClient(APP.BaseUrl);
            var projectId = APP.Dic["projectId"];
            var talbleId = APP.Dic["tableId"];

            var result = await client.SendAsync<object>($"/api/project/modelingTable/{projectId}/{talbleId}", Method.Get);

        }
        catch (Exception ex)
        {
            MessageBox.Show($"生成失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private class TextBoxWriter : TextWriter
    {
        private readonly System.Windows.Controls.TextBox _textBox;

        public TextBoxWriter(System.Windows.Controls.TextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            Write(value.ToString());
        }

        public override void Write(string? value)
        {
            if (value == null)
            {
                return;
            }
            // Ensure we update UI on the dispatcher thread
            _textBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                _textBox.AppendText(value);
                _textBox.ScrollToEnd();
            }), DispatcherPriority.Background);
        }

        public override void WriteLine(string? value)
        {
            Write((value ?? string.Empty) + Environment.NewLine);
        }
    }
}
