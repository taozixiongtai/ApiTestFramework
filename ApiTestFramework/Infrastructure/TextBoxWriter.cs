using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ApiTestFramework.Infrastructure;

/// <summary>
/// 将控制台输出到textbox中
/// </summary>
public class TextBoxWriter(TextBox textBox) : TextWriter
{

    public override Encoding Encoding => Encoding.UTF8;

    public override void WriteLine(string? value)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            textBox.AppendText(value + Environment.NewLine);
            textBox.ScrollToEnd();
        });
    }
}

