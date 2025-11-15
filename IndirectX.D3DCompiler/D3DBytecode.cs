using System;
using IndirectX.Interop;

namespace IndirectX.D3DCompiler;

public class D3DBytecode : Bytecode
{
    private bool _isDisposed;
    private Blob _blob;
    public ComPtr ComPtr => _blob.ComPtr;

    public D3DBytecode(ComPtr nativePtr, bool addRef = false) => _blob = new Blob(nativePtr, addRef);

    internal D3DBytecode(Blob blob) => _blob = blob;

    public Unknown AsUnknown() => _blob;

    public unsafe void* NativeVoidPtr => _blob.BufferPointer;

    public unsafe override nint NativePtr => (nint)_blob.BufferPointer;

    public override nint Length => _blob.BufferSize;

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        _isDisposed = true;

        if (disposing) _blob.Dispose();
    }
}
