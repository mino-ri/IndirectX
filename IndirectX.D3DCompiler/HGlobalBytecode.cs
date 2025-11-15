using System;
using System.Runtime.InteropServices;

namespace IndirectX.D3DCompiler;

public class HGlobalBytecode : Bytecode
{
    private bool _isDisposed;

    public HGlobalBytecode(int length)
    {
        NativePtr = Marshal.AllocHGlobal(length);
        Length = length;
    }

    public override nint NativePtr { get; }

    public override nint Length { get; }

    protected override void Dispose(bool disposing)
    {
        if (!_isDisposed) return;
        _isDisposed = true;

        Marshal.FreeHGlobal(NativePtr);
    }
}
