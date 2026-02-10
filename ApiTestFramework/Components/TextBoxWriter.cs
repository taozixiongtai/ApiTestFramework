using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ApiTestFramework.Components
{
    /// <summary>
    /// 将控制台输出到textbox中
    /// </summary>
    public class TextBoxWriter : TextWriter
    {
        private readonly TextBox _textBox;

        public TextBoxWriter(TextBox textBox)
        {
            _textBox = textBox;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine(string? value)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _textBox.AppendText(value + Environment.NewLine);
                _textBox.ScrollToEnd();
            });
        }
    }
}
