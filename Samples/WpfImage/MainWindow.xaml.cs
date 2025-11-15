using System;
using System.Windows;
using IndirectX.Helper;

namespace WpfImage;

public partial class MainWindow : Window
{
    private RenderLoop? _renderLoop;
    private TestRenderer? _testRenderer;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        _testRenderer = new TestRenderer(drawSurface);
        _renderLoop = new RenderLoop(_testRenderer.Frame);
        _renderLoop.Start();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _renderLoop?.Stop();
        _testRenderer?.Dispose();
    }
}
