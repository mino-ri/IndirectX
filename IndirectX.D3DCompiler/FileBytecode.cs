using System;
using System.IO;

namespace IndirectX.D3DCompiler;

public class FileBytecode : Bytecode
{
    private readonly FileStream _stream;
    private bool _isDisposed;

    public FileBytecode(FileStream stream) => _stream = stream;

    public FileBytecode(string path) : this(File.OpenRead(path)) { }

    public override nint NativePtr => _stream.SafeFileHandle.DangerousGetHandle();

    public override nint Length => checked((nint)_stream.Length);

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        _isDisposed = true;
        if (disposing) _stream.Dispose();
    }
}
