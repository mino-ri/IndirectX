using System;
using System.ComponentModel;
using IndirectX.Helper;
using IndirectX.Helper.Wpf;

namespace WpfCube.Models;

public class ViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly TestRenderer _renderer;
    private readonly RenderLoop _renderLoop;

    public ViewModel(DirectXHost host)
    {
        _renderer = new TestRenderer(host);
        _renderLoop = new RenderLoop(_renderer.Frame);
        _renderLoop.Start();
    }

    public FloatViewModel[] Parameters => _renderer.Parameters;

    public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

    public void Dispose()
    {
        _renderLoop.Stop();
        _renderer.Dispose();
    }
}
