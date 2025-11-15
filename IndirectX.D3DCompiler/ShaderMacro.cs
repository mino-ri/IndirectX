using System;
using IndirectX.Interop;
using System.Runtime.InteropServices;

namespace IndirectX.D3DCompiler;

[StructLayout(LayoutKind.Sequential)]
internal struct InteropShaderMacro
{
    public AnsiString Name;
    public AnsiString Definition;

    public InteropShaderMacro(AnsiString name, AnsiString definition)
    {
        Name = name;
        Definition = definition;
    }
}

internal unsafe readonly ref struct InteropShaderMacroArray
{
    public readonly InteropShaderMacro* NativePtr;
    private readonly int _length;

    public InteropShaderMacroArray(ReadOnlySpan<ShaderMacro> source)
    {
        if (source.Length == 0)
        {
            NativePtr = null;
            _length = 0;
        }
        else
        {
            _length = source.Length;
            NativePtr = (InteropShaderMacro*)Marshal.AllocHGlobal((_length + 1) * sizeof(InteropShaderMacro));
            for (var i = 0; i < _length; i++)
            {
                NativePtr[i].Name = new AnsiString(source[i].Name);
                NativePtr[i].Definition = new AnsiString(source[i].Definition);
            }
        }
    }

    public void Dispose()
    {
        for (var i = 0; i < _length; i++)
        {
            NativePtr[i].Name.Dispose();
            NativePtr[i].Definition.Dispose();
        }
        Marshal.FreeHGlobal((nint)NativePtr);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct ShaderMacro
{
    public string Name;
    public string Definition;

    public ShaderMacro(string name, string definition)
    {
        Name = name;
        Definition = definition;
    }
}
