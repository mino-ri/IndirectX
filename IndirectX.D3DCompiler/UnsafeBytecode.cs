using System;

namespace IndirectX.D3DCompiler;

public class UnsafeBytecode : Bytecode
{
    public override nint NativePtr { get; }

    public override nint Length { get; }

    public UnsafeBytecode(nint nativePtr, nint length) => (NativePtr, Length) = (nativePtr, length);

    public unsafe UnsafeBytecode(void* nativePtr, nint length) : this((nint)nativePtr, length) { }

    protected override void Dispose(bool disposing) { }
}
