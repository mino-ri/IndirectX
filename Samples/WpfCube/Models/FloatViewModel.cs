using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfCube.Models;

public abstract class FloatViewModel : INotifyPropertyChanged
{
    private readonly ChangeBox _hasChanged;

    public string Name { get; }

    public float Min { get; }

    public float Max { get; }

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

    protected FloatViewModel(string name, float min, float max, ChangeBox hasChanged)
    {
        _hasChanged = hasChanged;
        Name = name;
        Min = min;
        Max = max;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public static FloatViewModel Create<T>(string name, float min, float max, T container, RefFunc<T, float> getter, ChangeBox hasChanged) =>
        new Reference<T>(name, min, max, container, getter, hasChanged);

    public static FloatViewModel Create<T>(string name, float min, float max, T container, Func<T, float> getter, Action<T, float> setter, ChangeBox hasChanged) =>
        new GetSet<T>(name, min, max, container, getter, setter, hasChanged);

    private class Reference<T> : FloatViewModel
    {
        private readonly T _container;
        private readonly RefFunc<T, float> _getter;

        public Reference(string name, float min, float max, T container, RefFunc<T, float> getter, ChangeBox hasChanged)
            : base(name, min, max, hasChanged)
        {
            _container = container;
            _getter = getter;
        }

        protected override float ValueCore
        {
            get => _getter(_container);
            set => _getter(_container) = value;
        }
    }

    private class GetSet<T> : FloatViewModel
    {
        private readonly T _container;
        private readonly Func<T, float> _getter;
        private readonly Action<T, float> _setter;

        public GetSet(string name, float min, float max, T container, Func<T, float> getter, Action<T, float> setter, ChangeBox hasChanged)
            : base(name, min, max, hasChanged)
        {
            _container = container;
            _getter = getter;
            _setter = setter;
        }

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
