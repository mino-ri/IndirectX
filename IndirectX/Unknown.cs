using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using IndirectX.Interop;

namespace IndirectX;

[Guid("00000000-0000-0000-C000-000000000046")]
public abstract class Unknown : IDisposable
{
    private bool _isDisposed;
    public ComPtr ComPtr { get; private set; }

    protected Unknown(ComPtr nativePtr, bool addRef = false)
    {
        if (nativePtr.IsNull) throw new ArgumentException($"{nameof(nativePtr)} is null.", nameof(nativePtr));

        ComPtr = nativePtr;
        if (addRef) ComPtr.AddRef();
    }

    public T? QueryInterface<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
        where T : Unknown
    {
        return ComPtr.QueryInterface(GetIid<T>()) is ComPtr ptr && !ptr.IsNull
            ? Create<T>(ptr)
            : null;
    }

    public bool QueryInterface<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>([NotNullWhen(true)] out T? result)
        where T : Unknown
    {
        result = ComPtr.QueryInterface(GetIid<T>()) is ComPtr ptr && !ptr.IsNull
            ? Create<T>(ptr)
            : null;
        return result is not null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            ComPtr.Release();
            ComPtr = ComPtr.Null;
            _isDisposed = true;
        }
    }

    ~Unknown() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // AOTコンパイルの選択肢を増やすために提供しない
    // public static Guid GetIid(Type type) => type.GUID;

    public static Guid GetIid<T>() where T : Unknown => typeof(T).GUID;

    public static T? Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(ComPtr nativePtr, bool addRef = false) where T : Unknown =>
        nativePtr.IsNull
        ? null
        : UnknownActivator<T>.CreateInstance(nativePtr, addRef);
}
