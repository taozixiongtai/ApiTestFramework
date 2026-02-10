using ApiTestFramework.Components;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace ApiTestFramework;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        Console.SetOut(new TextBoxWriter(LogTextBox));
    }

    private async void GenerateSeedData_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("123");
    }

}
