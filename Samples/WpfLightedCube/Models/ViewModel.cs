using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using IndirectX.Helper;
using IndirectX.Helper.Wpf;

namespace WpfLightedCube.Models;

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
        GC.SuppressFinalize(this);
    }
}

public abstract class FloatViewModel(string name, float min, float max, ChangeBox hasChanged) : INotifyPropertyChanged
{
    private readonly ChangeBox _hasChanged = hasChanged;

    public string Name { get; } = name;

    public float Min { get; } = min;

    public float Max { get; } = max;

    public float Value
    {
        get => ValueCore;
        set
        {
            var newValue = Math.Clamp(value, Min, Max);
            if (ValueCore != newValue)
            {
                ValueCore = newValue;
                Interlocked.Exchange(ref _hasChanged.Value, 1);
                OnPropertyChanged();
            }
        }
    }

    protected abstract float ValueCore { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public static FloatViewModel Create<T>(string name, float min, float max, T container, RefFunc<T, float> getter, ChangeBox hasChanged) =>
        new Reference<T>(name, min, max, container, getter, hasChanged);

    public static FloatViewModel Create<T>(string name, float min, float max, T container, Func<T, float> getter, Action<T, float> setter, ChangeBox hasChanged) =>
        new GetSet<T>(name, min, max, container, getter, setter, hasChanged);

    private class Reference<T>(string name, float min, float max, T container, RefFunc<T, float> getter, ChangeBox hasChanged) : FloatViewModel(name, min, max, hasChanged)
    {
        private readonly T _container = container;
        private readonly RefFunc<T, float> _getter = getter;

        protected override float ValueCore
        {
            get => _getter(_container);
            set => _getter(_container) = value;
        }
    }

    private class GetSet<T>(string name, float min, float max, T container, Func<T, float> getter, Action<T, float> setter, ChangeBox hasChanged) : FloatViewModel(name, min, max, hasChanged)
    {
        private readonly T _container = container;
        private readonly Func<T, float> _getter = getter;
        private readonly Action<T, float> _setter = setter;

        protected override float ValueCore
        {
            get => _getter(_container);
            set => _setter(_container, value);
        }
    }
}

public delegate ref TResult RefFunc<in T, TResult>(T value);

public class ChangeBox
{
    public int Value;

    public bool IsChanged => Interlocked.Exchange(ref Value, 0) != 0;
}
