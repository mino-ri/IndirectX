using System.Runtime.InteropServices;

namespace IndirectX.Interop;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct InteropString
{
    public readonly nint NativePtr;

    public InteropString(int length) => NativePtr = Marshal.AllocHGlobal(length);

    public InteropString(nint length) => NativePtr = Marshal.AllocHGlobal((int)length);

    public InteropString(string? source) => NativePtr = Marshal.StringToHGlobalAnsi(source);

    public new string ToString() => Marshal.PtrToStringAnsi(NativePtr)!;

    public void Dispose() => Marshal.FreeHGlobal(NativePtr);

    public static implicit operator nint(InteropString str) => str.NativePtr;
}
