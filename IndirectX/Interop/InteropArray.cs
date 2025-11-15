using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace IndirectX.Interop;

/// <summary>For interop. DO NOT use in user code.</summary>
public unsafe readonly ref struct InteropArray<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>
    where T : Unknown
{
    public readonly ComPtr* NativePtr;
    public readonly int Length;

    public InteropArray(int length)
    {
        NativePtr = (ComPtr*)Marshal.AllocHGlobal(sizeof(ComPtr) * length);
        Length = length;
    }

    public InteropArray(T[] source)
    {
        NativePtr = (ComPtr*)Marshal.AllocHGlobal(sizeof(ComPtr) * source.Length);
        Length = source.Length;
        for (var i = 0; i < source.Length; i++)
            NativePtr[i] = source[i].ComPtr;
    }

    public T[] ToArray()
    {
        var result = new T[Length];
        for (var i = 0; i < Length; i++)
        {
            result[i] = Unknown.Create<T>(NativePtr[i])!;
        }

        return result;
    }

    public void Dispose() => Marshal.FreeHGlobal((nint)NativePtr);
}
