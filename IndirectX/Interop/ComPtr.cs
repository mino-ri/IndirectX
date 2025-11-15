using System.Runtime.InteropServices;

namespace IndirectX.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct ComPtr(nint nativePtr)
{
    public static readonly ComPtr Null = new(nint.Zero);

    public readonly nint Native = nativePtr;
    public bool IsNull => Native == nint.Zero;

    public int AddRef() => (**(UnknownVtbl**)Native).AddRef(Native);
    public int Release() => (**(UnknownVtbl**)Native).Release(Native);
    public void ReleaseIfNotNull()
    {
        if (!IsNull) Release();
    }
    public ComPtr? QueryInterface(in Guid guid)
    {
        return (**(UnknownVtbl**)Native).QueryInterface(Native, in guid, out var typedPtr) == HResult.Ok
            ? typedPtr
            : null;
    }
    public override string ToString() => Native.ToString();
    public override int GetHashCode() => Native.GetHashCode();

    public static implicit operator nint(ComPtr ptr) => ptr.Native;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct UnknownVtbl
{
    internal readonly delegate* unmanaged[Stdcall]<nint, in Guid, out ComPtr, HResult> QueryInterface;
    internal readonly delegate* unmanaged[Stdcall]<nint, int> AddRef;
    internal readonly delegate* unmanaged[Stdcall]<nint, int> Release;
}
