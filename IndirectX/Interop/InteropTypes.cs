using System.Runtime.InteropServices;

namespace IndirectX.Interop;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Bool(bool value)
{
    public readonly int Value = value ? 1 : 0;

    public static implicit operator Bool(bool value) => new Bool(value);
    public static implicit operator bool(Bool value) => value.Value != 0;
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct AnsiString(string str) : IDisposable
{
    public readonly nint Pointer = Marshal.StringToHGlobalAnsi(str);

    public void Dispose() => Marshal.FreeHGlobal(Pointer);

    //public static implicit operator AnsiString(string value) => value is null ? default : new AnsiString(value);
    public static implicit operator string?(AnsiString value) => Marshal.PtrToStringAnsi(value.Pointer);
}
