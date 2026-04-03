using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ApiTestFramework.UI.Infrastructure;

public class TextBoxWriter(TextBox textBox) : TextWriter
{

    public override Encoding Encoding => Encoding.UTF8;

    public override void WriteLine(string? value)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            textBox.AppendText(value + Environment.NewLine);
            textBox.ScrollToEnd();
        });
    }
}
