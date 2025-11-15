using System;
using System.Buffers;

namespace IndirectX.D3DCompiler;

public class ManagedBytecode : Bytecode
{
    private readonly MemoryHandle _memoryHandle;
    private bool _isDisposed;

    public ManagedBytecode(ReadOnlyMemory<byte> memory)
    {
        _memoryHandle = memory.Pin();
        Length = memory.Length;
    }

    public unsafe override nint NativePtr => (nint)_memoryHandle.Pointer;

    public override nint Length { get; }

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _memoryHandle.Dispose();
    }
}
