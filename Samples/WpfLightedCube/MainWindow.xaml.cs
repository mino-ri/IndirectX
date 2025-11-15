using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfLightedCube.Models;

namespace WpfLightedCube;

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
