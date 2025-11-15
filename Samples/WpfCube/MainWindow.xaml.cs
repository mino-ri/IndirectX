using System;
using System.Windows;
using WpfCube.Models;

namespace WpfCube;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ViewModel _viewModel = null!;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        _viewModel = new ViewModel(drawSurface);
        DataContext = _viewModel;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _viewModel.Dispose();
    }
}
